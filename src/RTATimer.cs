using System;
using TMPro;
using UnityEngine;

namespace EnhancedTimer
{
    /// <summary>
    /// Adds a speedrun.com RTA timer beside the game's IGT, styled to match the
    /// game's existing IGT timer.
    ///
    /// RTA start: first frame of movement input, OR
    ///            firing the rocket.
    /// RTA stop:  the frame the results screen appears.
    /// </summary>
    public class RTATimer : MonoBehaviour
    {
        private enum State
        {
            WaitingForStart,
            Running,
            Finished
        }

        private State _state = State.WaitingForStart;
        private double _accumulated;
        private double _final;
        private TextMeshProUGUI _text;
        private GameObject _resultsCanvas;
        private bool _positioned;

        public static double FinalRTA { get; private set; }
        public static double CurrentRTA { get; private set; }

        public static bool EnsureCreated()
        {
            var timerGo = GameObject.Find("Timer");
            if (timerGo == null)
                return false;

            var existing = GameObject.Find("RTA Timer");
            if (existing != null)
                return true;

            var parent = timerGo.transform.parent;
            var cloneGo = Instantiate(timerGo, parent);
            cloneGo.name = "RTA Timer";

            var label = cloneGo.GetComponent<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = "0.0";
                label.enableWordWrapping = false;
                label.overflowMode = TextOverflowModes.Overflow;
            }

            cloneGo.AddComponent<RTATimer>();
            return true;
        }

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            _resultsCanvas = GameObject.Find("GameOver Canvas");
            _state = State.WaitingForStart;
        }

        private void LateUpdate()
        {
            if (!_positioned)
            {
                _positioned = true;
                PositionBelowSource();
            }

            switch (_state)
            {
                case State.WaitingForStart:
                    bool move =
                        Mathf.Abs(Input.GetAxis("Horizontal")) > 0.0001f ||
                        Mathf.Abs(Input.GetAxis("Vertical")) > 0.0001f;
                    bool fire = Input.GetButtonDown("Fire1");
                    if (move || fire)
                    {
                        _accumulated = 0.0;
                        _state = State.Running;
                    }
                    break;

                case State.Running:
                    if (_resultsCanvas == null)
                        _resultsCanvas = GameObject.Find("GameOver Canvas");

                    if (_resultsCanvas != null && _resultsCanvas.activeSelf)
                    {
                        _final = _accumulated;
                        FinalRTA = _final;
                        CurrentRTA = _final;
                        _state = State.Finished;
                        UpdateText(_final);
                    }
                    else
                    {
                        _accumulated += Time.unscaledDeltaTime;
                        CurrentRTA = _accumulated;
                        UpdateText(_accumulated);
                    }
                    break;

                case State.Finished:
                    break;
            }
        }

        private void PositionBelowSource()
        {
            var timerGo = GameObject.Find("Timer");
            if (timerGo == null)
                return;

            var srcRect = timerGo.GetComponent<RectTransform>();
            var dstRect = GetComponent<RectTransform>();
            if (srcRect == null || dstRect == null)
                return;

            dstRect.anchorMin = srcRect.anchorMin;
            dstRect.anchorMax = srcRect.anchorMax;
            dstRect.pivot = srcRect.pivot;
            dstRect.sizeDelta = new Vector2(Mathf.Max(srcRect.rect.width, 160f), srcRect.rect.height);
            dstRect.anchoredPosition = srcRect.anchoredPosition
                + new Vector2(0f, -srcRect.rect.height - 2f);
        }

        private void UpdateText(double seconds)
        {
            if (_text == null)
                return;

            var ts = TimeSpan.FromSeconds(seconds);
            string formatted;
            if (ts.TotalHours >= 1.0)
                formatted = string.Format("{0}:{1:00}:{2:00}.{3:000}",
                    (int)ts.TotalHours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            else
                formatted = string.Format("{0}:{1:00}.{2:000}",
                    ts.Minutes, ts.Seconds, ts.Milliseconds);

            _text.text = formatted;
        }
    }
}