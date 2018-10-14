using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Derived from: https://github.com/genecyber/Mumbles/blob/master/Suicide-note.g
public class SuicideNoteGenerator : MonoBehaviour
{
    public abstract class MultiValuedObject
    {
        public abstract string[] Values { get; set; }

        public string Value
        {
            get
            {
                return Values[UnityEngine.Random.Range(0, Values.Length)];
            }
        }
    }

    [Serializable]
    public class Template : MultiValuedObject
    {
        public string Name;

        public override string[] Values { get; set; }
    }

    public Text SuicideNodeText;

    private static readonly Dictionary<SuicideNoteIntention, string[]> Salutations = new Dictionary<SuicideNoteIntention, string[]>()
    {
        {
            SuicideNoteIntention.RevengeAgainstRecipient,
            new string[] 
            {
                "Dear %insult% %recipient%,",
                "To my %insult% %recipient%,"
            }
        },

        {
            SuicideNoteIntention.RevengeAgainstTheWorld,
            new string[]
            {
                "To all %insult% people out there,",
                "Oh %insult% humanity,"
            }
        },

        {
            SuicideNoteIntention.SeekingAttention,
            new string[]
            {
                "Oh, %adjective% world,",
                "To whom it may concern,"
            }
        }
    };

    private static readonly Dictionary<SuicideNoteIntention, string[]> Motivations = new Dictionary<SuicideNoteIntention, string[]>()
    {
        {
            SuicideNoteIntention.RevengeAgainstRecipient,
            new string[]
            {
                "You made me see that %reason%.",
                "You made me realise that %reason%.",
                "All those years by your side made me realise that %reason%.",
                "Because of you, %reason%.",
                "Exclusively because of you, %reason%."
            }
        },

        {
            SuicideNoteIntention.RevengeAgainstTheWorld,
            new string[]
            {
                "You all made me see that %reason%.",
                "Influenced by all of you, %reason%."
            }
        },

        {
            SuicideNoteIntention.SeekingAttention,
            new string[]
            {
                "Love is pain, existence is pointless.",
                "I never had the admiration I wanted in life... maybe things will be better in death."
            }
        }
    };

    private static readonly Dictionary<SuicideNoteIntention, string[]> Closings = new Dictionary<SuicideNoteIntention, string[]>()
    {
        {
            SuicideNoteIntention.RevengeAgainstRecipient,
            new string[]
            {
                "May I rest in peace.",
                "See you in the next life.",
                "See you."
            }
        },

        {
            SuicideNoteIntention.RevengeAgainstTheWorld,
            new string[]
            {
                "Fuck you, %insult% world.",
                "See you in Hell."
            }
        },

        {
            SuicideNoteIntention.SeekingAttention,
            new string[]
            {
                "Goodbye.",
                "See you in Heaven.",
                "My destiny awaits."
            }
        }
    };

    private static readonly Template[] Templates = new Template[]
    {
        new Template()
        {
            Name = "adjective",
            Values = new string[]
            {
                "boring",
                "cheerless",
                "cruel",
                "depressing",
                "desolate",
                "despicable",
                "devilish",
                "dreary",
                "dull",
                "harsh",
                "heartless",
                "lonely",
                "mean",
                "miserable",
                "monotonous",
                "pitiful",
                "relentless",
                "satanic",
                "savage",
                "tedious",
                "tiresome",
                "uncaring",
                "unhappy",
                "unkind",
                "wearisome",
                "weary",
                "worthless",
                "wretched"
            }
        },
        new Template()
        {
            Name = "insult",
            Values = new string[]
            {
                "fucked-up",
                "despicable",
                "loathsome",
                "abject",
                "wretched",
                "pitiful",
                "disgraceful",
                "vile"
            }
        },
        new Template()
        {
            Name = "reason",
            Values = new string[]
            {
                "hell sounds interesting",
                "hell would be more fun than this place",
                "death sounds like fun",
                "I like living on the edge",
                "life is not worth living",
                "life is boring",
                "the voices in my head tell me to kill myself",
                "I have learned everything there is to learn here",
                "between Heaven and Hell, I choose the latter"
            }
        }
    };

    public void Start()
    {
        SuicideNote.Instance.RandomiseContent();
        GenerateText();
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            SceneManager.LoadScene("Falling");
        }
    }

    private void GenerateText()
    {
        var @string = ReplaceTemplates(Salutations[SuicideNote.Instance.Intention]) + Environment.NewLine;
        @string += ReplaceTemplates(Motivations[SuicideNote.Instance.Intention]) + Environment.NewLine;
        @string += ReplaceTemplates(Closings[SuicideNote.Instance.Intention]);
        SuicideNodeText.text = @string;
    }

    private string ReplaceTemplates(string[] values)
    {
        var @string = values[UnityEngine.Random.Range(0, values.Length)];
        @string = ReplaceDynamicTemplates(@string);
        @string = ReplaceStaticTemplates(@string);
        return @string;
    }

    private string ReplaceStaticTemplates(string @string)
    {
        return Regex.Replace(@string, $"%recipient%", SuicideNote.Instance.Recipient.ToString(), RegexOptions.Multiline | RegexOptions.IgnoreCase);
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
}
