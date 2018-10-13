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

    [System.Flags]
    public enum Intention
    {
        RevengeAgainstRecipient,
        RevengeAgainstTheWorld,
        SeekingAttention
    }

    public enum Recipient : uint
    {
        Undefined = 1,
        Mom,
        Dad,
        Son,
        Daughter,
        Wife,
        Husband,
        Friend,
        Boss
    }

    public static Intention NoteIntention { get; private set; }

    public static Recipient NoteRecipient { get; private set; }

    public Text SuicideNodeText;

    private static readonly Dictionary<Intention, string[]> Salutations = new Dictionary<Intention, string[]>()
    {
        {
            Intention.RevengeAgainstRecipient,
            new string[] 
            {
                "Dear %insult% %recipient%,",
                "To my %insult% %recipient%,"
            }
        },

        {
            Intention.RevengeAgainstTheWorld,
            new string[]
            {
                "To all %insult% people out there,",
                "Oh %insult% humanity,"
            }
        },

        {
            Intention.SeekingAttention,
            new string[]
            {
                "Oh, %adjective% world,",
                "To whom it may concern,"
            }
        }
    };

    private static readonly Dictionary<Intention, string[]> Motivations = new Dictionary<Intention, string[]>()
    {
        {
            Intention.RevengeAgainstRecipient,
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
            Intention.RevengeAgainstTheWorld,
            new string[]
            {
                "You all made me see that %reason%.",
                "Influenced by all of you, %reason%."
            }
        },

        {
            Intention.SeekingAttention,
            new string[]
            {
                "Love is pain, existence is pointless.",
                "I never had the admiration I wanted in life... maybe things will be better in death."
            }
        }
    };

    private static readonly Dictionary<Intention, string[]> Closings = new Dictionary<Intention, string[]>()
    {
        {
            Intention.RevengeAgainstRecipient,
            new string[]
            {
                "May I rest in peace.",
                "See you in the next life.",
                "See you."
            }
        },

        {
            Intention.RevengeAgainstTheWorld,
            new string[]
            {
                "Fuck you, %insult% world.",
                "See you in Hell."
            }
        },

        {
            Intention.SeekingAttention,
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
        RandomiseContent();
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
        var @string = ReplaceTemplates(Salutations[NoteIntention]) + Environment.NewLine;
        @string += ReplaceTemplates(Motivations[NoteIntention]) + Environment.NewLine;
        @string += ReplaceTemplates(Closings[NoteIntention]);
        SuicideNodeText.text = @string;
    }

    private void RandomiseContent()
    {
        var intentions = Enum.GetValues(typeof(Intention));
        NoteIntention = (Intention)intentions.GetValue(UnityEngine.Random.Range(0, intentions.Length));

        if (NoteIntention == Intention.RevengeAgainstRecipient)
        {
            var recipients = Enum.GetValues(typeof(Recipient));
            NoteRecipient = (Recipient)recipients.GetValue(UnityEngine.Random.Range(1, recipients.Length));
        }
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
}
