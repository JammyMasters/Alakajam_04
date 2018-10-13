using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

// Derived from: https://github.com/genecyber/Mumbles/blob/master/Suicide-note.g
public class SuicideNote : MonoBehaviour, IGameStateObserver
{
    public abstract class AssetBasedRandomValueObject
    {
        public TextAsset Asset;

        private string[] m_parsedValues;

        private string[] Values
        {
            get
            {
                if (m_parsedValues == null)
                {
                    m_parsedValues = Asset.text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                }
                return m_parsedValues;
            }
        }

        public string Value
        {
            get
            {
                return Values[UnityEngine.Random.Range(0, Values.Length)];
            }
        }
    }

    [Serializable]
    public class Sections : AssetBasedRandomValueObject
    {
        [EnumFlags]
        public Intention Intentions;

        // Source: https://forum.unity.com/threads/multiple-enum-select-from-inspector.184729/
        public List<Intention> ApplicableIntentions
        {
            get
            {
                var applicableGoals = new List<Intention>();
                var goal = Enum.GetValues(typeof(Intention));
                for (int i = 0; i < goal.Length; i++)
                {
                    int layer = 1 << i;
                    if (((int)Intentions & layer) != 0)
                    {
                        applicableGoals.Add((Intention)goal.GetValue(i));
                    }
                }
                return applicableGoals;
            }
        }
    }

    [Serializable]
    public class Template : AssetBasedRandomValueObject
    {
        public string Name;
    }

    [System.Flags]
    public enum Intention
    {
        RevengeAgainstRecipient,
        RevengeAgainstTheWorld,
        SeekingAttention
    }

    [System.Flags]
    public enum Recipient
    {
        Undefined,
        Mom,
        Dad,
        Son,
        Daughter,
        Wife,
        Husband,
        Friend,
        Boss
    }

    public Intention NoteIntention { get; private set; }

    public Recipient NoteRecipient { get; private set; }

    public Text SuicideNodeText;

    public Sections[] Salutations;

    public Sections[] Motivations;

    public Sections[] Closings;

    public Template[] Templates;

    private void RandomiseContent()
    {
        var intentions = Enum.GetValues(typeof(Intention));
        NoteIntention = (Intention)intentions.GetValue(UnityEngine.Random.Range(0, intentions.Length));

        if (NoteIntention == Intention.RevengeAgainstRecipient)
        {
            var recipients = Enum.GetValues(typeof(Recipient));
            NoteRecipient = (Recipient)recipients.GetValue(UnityEngine.Random.Range(0, recipients.Length));
        }
    }

    public void OnEnterState(GameState state)
    {
        if (state != GameState.SuicideNote)
        {
            return;
        }

        RandomiseContent();

        var @string = GetSectionString(Salutations) + Environment.NewLine;
        @string += GetSectionString(Motivations) + Environment.NewLine;
        @string += GetSectionString(Closings);
        SuicideNodeText.text = @string;
    }

    private string GetSectionString(IEnumerable<Sections> elements)
    {
        var section = elements.Where(x => x.ApplicableIntentions.Contains(NoteIntention)).ToArray();
        if (section.Length == 0)
        {
            return string.Empty;
        }
        var @string = section[UnityEngine.Random.Range(0, section.Length)].Value;
        @string = ReplaceDynamicTemplates(@string);
        @string = ReplaceStaticTemplates(@string);
        return @string;
    }

    private string ReplaceStaticTemplates(string @string)
    {
        return Regex.Replace(@string, $"%recipient%", NoteRecipient.ToString(), RegexOptions.Multiline | RegexOptions.IgnoreCase);
    }

    private string ReplaceDynamicTemplates(string @string)
    {
        foreach (var template in Templates)
        {
            @string = ReplaceTemplates(@string, template);
        }
        return @string;
    }

    private string ReplaceTemplates(string @string, Template template)
    {
        if (string.IsNullOrEmpty(@string))
        {
            return string.Empty;
        }

        return Regex.Replace(@string, $"%{template.Name}%", template.Value, RegexOptions.Multiline | RegexOptions.IgnoreCase);
    }

    public void OnLeaveState(GameState state)
    {
    }
}
