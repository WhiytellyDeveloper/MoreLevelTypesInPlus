using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace MoreLevelTypesInPlus.Structures.Objects
{
    public class Spikes : Door
    {
        public override void Initialize()
        {
            base.Initialize();
            Block(true);
            spikesList.Add(this);
        }

        void Update()
        {
            if (spikeFreind == null || spikeFreind == this) return;

            int current = Singleton<BaseGameManager>.Instance.FoundNotebooks;
            if (current == lastNotebookCount || GetInstanceID() > spikeFreind.GetInstanceID()) return;

            lastNotebookCount = spikeFreind.lastNotebookCount = current;
            bool openSelf = new System.Random(Singleton<CoreGameManager>.Instance.Seed() + current).Next(0, 100) >= 50;

            SetState(openSelf);
            spikeFreind.SetState(!openSelf);

            for (int x = 0; x < ec.map.tiles.GetLength(0); x++)
            {
                for (int y = 0; y < ec.map.tiles.GetLength(1); y++)
                {
                    MapTile tile = ec.map.tiles[x, y];
                    IntVector2 pos = new(x, y);

                    bool isMyTile = (pos == aTile.position || pos == bTile.position);
                    bool isFriendTile = spikeFreind != null && (pos == spikeFreind.aTile.position || pos == spikeFreind.bTile.position);

                    if (isMyTile || isFriendTile)
                    {
                        bool myTilesFound = tile.Found && ec.map.tiles[spikeFreind.aTile.position.x, spikeFreind.aTile.position.z].Found || ec.map.tiles[spikeFreind.bTile.position.x, spikeFreind.bTile.position.z].Found;
                        
                        if (myTilesFound)  tile.Reveal();     
                    }
                }
            }
        }

        void SetState(bool open)
        {
            if (isOpen == open) return;
            isOpen = open;
            if (open) Open(false, false); else Shut();
        }

        public override void Shut() => AnimateSpikes(-0.8f + 5f, false);
        public override void Open(bool cancelTimer, bool makeNoise) => AnimateSpikes(-3f, true);

        void AnimateSpikes(float y, bool trigger)
        {
            if (moveRoutine != null) StopCoroutine(moveRoutine);
            moveRoutine = StartCoroutine(MoveSpikesRoutine(y, trigger));
        }

        IEnumerator MoveSpikesRoutine(float targetY, bool trigger)
        {
            audMan.PlaySingle(trigger ? spikesUp : spikesDown);

            while (true)
            {
                bool done = true;
                foreach (var t in _spikeTransforms)
                {
                    float ny = Mathf.MoveTowards(t.localPosition.y, targetY, animationSpeed * ec.EnvironmentTimeScale * Time.deltaTime);
                    if (!Mathf.Approximately(t.localPosition.y, targetY)) done = false;
                    t.localPosition = new Vector3(t.localPosition.x, ny, t.localPosition.z);
                }
                if (done) break;
                yield return null;
            }

            SetBlock(!trigger);
            _collider.isTrigger = trigger;
            icon.SpriteRenderer.sprite = !trigger ? upIconSprite : downIconSprite;
        }

        void SetBlock(bool blocked)
        {
            ec.FreezeNavigationUpdates(true);
            Block(blocked);
            ec.FreezeNavigationUpdates(false);
        }

        public override void UnInitialize()
        {
            Block(false);
            Open(false, false);
            spikesList.Remove(this);
            base.UnInitialize();
        }

        public static void PairAllSpikes(List<Spikes> spikesList, EnvironmentController ec)
        {
            List<Spikes> unpairedSpikes = spikesList.Where(s => s.spikeFreind == null).ToList();
            Dictionary<(Spikes a, Spikes b), float> distances = new();

            // Etapa 1: calcular distâncias entre todos os pares
            foreach (var spike in unpairedSpikes)
            {
                DijkstraMap dijkstra = new DijkstraMap(ec, PathType.Const, int.MaxValue, Array.Empty<Transform>());
                dijkstra.Calculate(int.MaxValue, false, [spike.aTile.position]);
                dijkstra.pathType = PathType.Nav;

                foreach (var other in unpairedSpikes)
                {
                    if (spike == other) continue;
                    float dist = dijkstra.Value(other.aTile.position);
                    if (!float.IsInfinity(dist))
                    {
                        distances[(spike, other)] = dist;
                    }
                }
            }

            // Etapa 2: encontrar pares mutuamente mais próximos
            HashSet<Spikes> paired = new();

            foreach (var spike in unpairedSpikes)
            {
                if (paired.Contains(spike)) continue;

                var closest = distances
                    .Where(kv => kv.Key.a == spike && !paired.Contains(kv.Key.b))
                    .OrderBy(kv => kv.Value)
                    .Select(kv => kv.Key.b)
                    .FirstOrDefault();

                if (closest == null || paired.Contains(closest)) continue;

                // Verifica se o spike também é o mais próximo do outro
                var closestToOther = distances
                    .Where(kv => kv.Key.a == closest && !paired.Contains(kv.Key.b))
                    .OrderBy(kv => kv.Value)
                    .Select(kv => kv.Key.b)
                    .FirstOrDefault();

                if (closestToOther == spike)
                {
                    // Pareamento mutuo
                    spike.spikeFreind = closest;
                    closest.spikeFreind = spike;

                    spike.SetState(false);
                    closest.SetState(true);

                    Color sharedColor = ColorUtilityHelper.GenerateDistinctColor(usedColors);
                    usedColors.Add(sharedColor);
                    spike.sharedColor = sharedColor;
                    closest.sharedColor = sharedColor;

                    spike.ApplyColorTile(sharedColor);
                    closest.ApplyColorTile(sharedColor);

                    paired.Add(spike);
                    paired.Add(closest);
                }
            }

            // Etapa 3: os que não foram pareados ficam sozinhos
            foreach (var spike in unpairedSpikes)
            {
                if (!paired.Contains(spike))
                {
                    spike.SetState(true);
                    Color sharedColor = ColorUtilityHelper.GenerateDistinctColor(usedColors);
                    usedColors.Add(sharedColor);
                    spike.sharedColor = sharedColor;
                    spike.ApplyColorTile(sharedColor);
                }
            }
        }


        private void ApplyColorTile(Color color)
        {
            icon = ec.map.AddExtraTile(IntVector2.GetGridPosition(transform.position));
            icon.transform.rotation = direction.ToUiRotation();
            icon.spriteRenderer.color = color;
            icon.spriteRenderer.sprite = upIconSprite;
        }


        public BoxCollider _collider;
        public List<Transform> _spikeTransforms = new();
        public PropagatedAudioManager audMan;
        public SoundObject spikesUp, spikesDown;
        public float animationSpeed = 62f;

        public static List<Spikes> spikesList = new();
        public Spikes spikeFreind;

        private Coroutine moveRoutine;
        private bool isOpen;
        private int lastNotebookCount = -1;

        private static List<Color> usedColors = new();
        private Color sharedColor = Color.white;
        public MapTile icon;
        public Sprite upIconSprite, downIconSprite;

    }
}

static class ColorUtilityHelper
{
    public static Color GenerateDistinctColor(List<Color> existingColors, float minDistance = 0.25f, int maxAttempts = 100)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Color newColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.6f, 1f);
            bool distinct = true;

            foreach (Color color in existingColors)
            {
                if (Vector3.Distance(new(newColor.r, newColor.g, newColor.b), new(color.r, color.g, color.b)) < minDistance)
                {
                    distinct = false;
                    break;
                }
            }

            if (distinct) return newColor;
        }

        return Color.white;
    }
}
