using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Avalonia.Media;
using Polygons.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Polygons;

public class ShapeConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Shape);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        JObject json = JObject.Load(reader);
        switch (json["Type"]!.Value<string>())
        {
            case "Circle":
                return json.ToObject<Circle>(serializer);
            case "Triangle":
                return json.ToObject<Triangle>(serializer);
            case "Square":
                return json.ToObject<Square>(serializer);
            default:
                return null;
        }
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) { }
}

[Serializable]
public class Data
{
    public List<Shape> Shapes { get; set; }
    public double R { get; set; }

    public Color Color { get; set; }
    public Data(List<Shape> shapes, double r, Color color)
    {
        Shapes = shapes;
        R = r;
        Color = color;
    }
}

public abstract class FileSaver
{
    public static void SaveFile(CustomControl customControl, string filename)
    {
        Data data = new Data(customControl.Shapes, Shape.R, Shape.color);
        string json = JsonConvert.SerializeObject(data);
        File.WriteAllText(filename, json);
    }

    public static void OpenFile(CustomControl customControl, string json)
    {
        JsonConverter[] converters = [new ShapeConverter()];
        Data? data =
            JsonConvert.DeserializeObject<Data>(json, new JsonSerializerSettings { Converters = converters });
        Debug.Assert(data != null, nameof(data) + " != null");
        customControl.Shapes = data.Shapes;
        Shape.R = data.R;
        Shape.color = data.Color;
    }
}