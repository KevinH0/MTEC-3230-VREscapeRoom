using JustAssets.Shared.UI.Animations;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    [CustomEditor(typeof(AnimatedUI), true)]
    public class AnimatedUIEditor : UnityEditor.Editor
    {
        private AnimatedUI _ui;

        private Animation _animation;

        public AnimatedUI UI
        {
            get => _ui;
            private set
            {
                if (_ui == value)
                    return;

                _ui = value;
                _animation = _ui.gameObject.GetComponent<Animation>();
            }
        }

        public override void OnInspectorGUI()
        {
            UI = (AnimatedUI) target;

            EditorGUILayout.LabelField("Current state:", UI.State.ToString());

            EditorGUILayout.HelpBox(
                UI.UseAnimator
                    ? "Set a trigger called Show and Hide in your animator graph to transition between open and closed state."
                    : "Place the animation to play for showing the dialog in slot 0, the one for hiding in slot 1 and the idle animation while visible in slot 2 (optional)", MessageType.Info);

            if (GUILayout.Button("Show"))
                UI.Show();

            if (GUILayout.Button("Hide"))
                UI.Hide();

            if (!UI.UseAnimator)
            {
                for (int i = 0; i < UI.AnimationsClips.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    AnimationClip uiAnimationsClip = UI.AnimationsClips[i];
                    UI.AnimationsClips[i] = (AnimationClip)EditorGUILayout.ObjectField(uiAnimationsClip, typeof(AnimationClip), false);

                    if (GUILayout.Button("▶", GUILayout.Width(30)))
                        UI.Play(uiAnimationsClip);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginVertical(EditorStyles.textArea);
                    {
                        if (_animation != null)
                            foreach (AnimationState state in _animation)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    string paused = state.enabled ? "▶" : "||";
                                    EditorGUILayout.LabelField($"Speed [{paused}]: {state.speed:P0}", GUILayout.Width(100));
                                    EditorGUILayout.Slider($"[{state.weight:P0}] {state.name.Substring(0, Mathf.Min(state.name.Length, 10))}", state.time, 0,
                                        state.length);
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                    }
                    EditorGUILayout.EndVertical();
                }
            }

            if (UI.InfluencedByLayout)
                EditorGUILayout.HelpBox("This UI will deactivate the layout during animation.", MessageType.Warning);
            
            DrawDefaultInspector();
        }
    }
}