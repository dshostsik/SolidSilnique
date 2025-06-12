using System.Collections.Generic;
using System.Xml.Linq;

public class Note
{
    public double Time { get; set; }
    public int Button { get; set; }
}

public static class NotesLoader
{
    public static List<Note> LoadNotesFromXml(string xmlFilePath)
    {
        var notes = new List<Note>();
        

        XDocument doc = XDocument.Load(xmlFilePath);

        foreach (var noteElem in doc.Descendants("Note"))
        {
            double time = double.Parse(noteElem.Element("time")?.Value ?? "0", System.Globalization.CultureInfo.InvariantCulture);
            int button = int.Parse(noteElem.Element("button")?.Value ?? "0");

            notes.Add(new Note
            {
                Time = time,
                Button = button
            });
        }

        return notes;
    }
}