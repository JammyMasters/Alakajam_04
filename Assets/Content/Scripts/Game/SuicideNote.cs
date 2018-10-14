using System;

public enum SuicideNoteIntention : uint
{
    RevengeAgainstRecipient = 1,
    RevengeAgainstTheWorld,
    SeekingAttention
}

public enum SuicideNoteRecipient : uint
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

public class SuicideNote
{
    private static SuicideNote s_instance;

    public static SuicideNote Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new SuicideNote();
            }
            return s_instance;
        }
    }

    public SuicideNote()
    {
        RandomiseContent();
    }

    public void RandomiseContent()
    {
        var allIntentions = Enum.GetValues(typeof(SuicideNoteIntention));
        Intention = (SuicideNoteIntention)allIntentions.GetValue(UnityEngine.Random.Range(0, allIntentions.Length));

        if (Intention == SuicideNoteIntention.RevengeAgainstRecipient)
        {
            var allRecipients = Enum.GetValues(typeof(SuicideNoteRecipient));
            Recipient = (SuicideNoteRecipient)allRecipients.GetValue(UnityEngine.Random.Range(1, allRecipients.Length));
        }
    }

    public SuicideNoteIntention Intention { get; private set; }

    public SuicideNoteRecipient Recipient { get; private set; }

}
