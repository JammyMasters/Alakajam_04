using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

// Derived from: https://github.com/genecyber/Mumbles/blob/master/Suicide-note.g
public class SuicideNoteGenerator : MonoBehaviour, IGameStateObserver
{
    public abstract class TextAssetBasedMultiValuedObject
    {
        public TextAsset Text;

        public string[] Values
        {
            get
            {
                return Text.text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            }
        }
    }

    [Serializable]
    public class SuicideNoteElements : TextAssetBasedMultiValuedObject
    {
        [EnumFlags]
        public PlayerGoal Goals;

        // Source: https://forum.unity.com/threads/multiple-enum-select-from-inspector.184729/
        public List<PlayerGoal> ApplicableGoals
        {
            get
            {
                var applicableGoals = new List<PlayerGoal>();
                var playerGoals = Enum.GetValues(typeof(PlayerGoal));
                for (int i = 0; i < playerGoals.Length; i++)
                {
                    int layer = 1 << i;
                    if (((int)Goals & layer) != 0)
                    {
                        applicableGoals.Add((PlayerGoal)playerGoals.GetValue(i));
                    }
                }
                return applicableGoals;
            }
        }
    }

    [Serializable]
    public class Template : TextAssetBasedMultiValuedObject
    {
        public string Name;
    }

    public PlayerGoal m_playerGoal;

    public Text SuicideNodeText;

    public SuicideNoteElements[] PerGoalSalutations;

    public SuicideNoteElements[] PerGoalMotivations;

    public SuicideNoteElements[] PerGoalClosings;

    public Template[] Templates;

    private void RandomiseGoal()
    {
        var playerGoals = Enum.GetValues(typeof(PlayerGoal));
        m_playerGoal = (PlayerGoal)playerGoals.GetValue(UnityEngine.Random.Range(0, playerGoals.Length));
    }

    public void OnEnterState(GameState state)
    {
        if (state != GameState.SUICIDE_NOTE)
        {
            return;
        }

        RandomiseGoal();

        var suicideNote = GenerateSalutation() + Environment.NewLine;
        suicideNote += GenerateMotivation() + Environment.NewLine;
        suicideNote += GenerateClosing();
        SuicideNodeText.text = suicideNote;
    }

    private string GenerateSection(IEnumerable<SuicideNoteElements> elements)
    {
        var values = elements.Where(x => x.ApplicableGoals.Contains(m_playerGoal)).SelectMany(x => x.Values).ToArray();
        if (values.Length == 0)
        {
            return string.Empty;
        }
        return RandomlyReplaceTemplates(values[UnityEngine.Random.Range(0, values.Length)]);
    }

    private string RandomlyReplaceTemplates(string @string)
    {
        foreach (var template in Templates)
        {
            @string = RandomlyReplaceTemplates(@string, template.Name, template.Values);
        }
        return @string;
    }

    private string RandomlyReplaceTemplates(string @string, string templateName, string[] templateValues)
    {
        if (string.IsNullOrEmpty(@string))
        {
            return string.Empty;
        }

        if (templateValues.Length == 0)
        {
            return @string;
        }

        return Regex.Replace(@string, $"%{templateName}%", templateValues[UnityEngine.Random.Range(0, templateValues.Length)], RegexOptions.Multiline | RegexOptions.IgnoreCase);
    }

    private string GenerateClosing()
    {
        return GenerateSection(PerGoalClosings);
    }

    private string GenerateMotivation()
    {
        return GenerateSection(PerGoalMotivations);
    }

    private string GenerateSalutation()
    {
        return GenerateSection(PerGoalSalutations);
    }

    public void OnLeaveState(GameState state)
    {
    }
}
