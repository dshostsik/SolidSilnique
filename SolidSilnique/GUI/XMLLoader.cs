using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace GUIRESOURCES;

public static class XMLLoader
{
    

    public static List<GuiElement> readUIFile(string filename)
    {
        List<GuiElement> elements = new List<GuiElement>();
        FileStream XMLFile = new FileStream(filename, FileMode.Open);
        XmlDocument UIFile = new XmlDocument();
        UIFile.Load(XMLFile);
        XmlElement root = UIFile.DocumentElement;
        XmlNodeList nodes = root.GetElementsByTagName("GuiElement");

        foreach (XmlNode node in nodes)
        {
            elements.Add(resolveElement(node));
        }
        return elements;
    }

    private static GuiElement resolveElement(XmlNode node)
    {
        GuiElement element = new GuiElement();
        string? amog = node.FirstChild.Name;
        float PositionX = float.Parse(node.Attributes["positionX"].Value);
        float PositionY = float.Parse(node.Attributes["positionY"].Value);
        string name = node.FirstChild.Attributes["name"].Value;
        float scale = float.Parse(node.Attributes["scale"].Value);
        switch (amog)
        {
            case "Bar":
                bool orientation = node.FirstChild.Attributes["orientation"].Value.Equals("vertical");
                bool inverted = node.FirstChild.Attributes["inverse"].Value.Equals("true");
                element = new ProgressBar(PositionX, PositionY,name,orientation,inverted,scale);
                break;
            case "Button":
                float width2 = float.Parse(node.FirstChild.Attributes["width"].Value);
                float height = float.Parse(node.FirstChild.Attributes["height"].Value);
                element = new Button(PositionX,PositionY,name,width2,height,scale);
                break;
            case "Image":
                element = new Image(PositionX, PositionY, name,scale);
                break;
            case "TextInput":
                float fontSize = float.Parse(node.FirstChild.Attributes["fontSize"].Value);
                string font = node.FirstChild.Attributes["fontStyle"].Value;
                string placeHolder = node.FirstChild.Attributes["placeholder"].Value;
                element = new TextInput(PositionX,PositionY,name,fontSize,font,placeHolder,scale);
                break;
            case "Text":
                float fontSize2 = float.Parse(node.FirstChild.Attributes["fontSize"].Value);
                string font2 = node.FirstChild.Attributes["fontStyle"].Value;
                string placeHolder2 = node.FirstChild.InnerText;
                element = new Text(PositionX,PositionY,name,placeHolder2,fontSize2,font2,scale);
                break;
            default:
                Console.WriteLine("Unknown element");
                break;
        }
        return element;
    }
    
}