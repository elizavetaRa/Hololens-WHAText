using UnityEngine;

/// <summary>
/// Represents result of an OCR
/// </summary>
public class OcrResult {

    public OcrResult (string Text, Rect BoundingBox)
    {
        this.Text = Text;
        this.BoundingBox = BoundingBox;
        Words = Text.Split(' ');
    }

    public Rect BoundingBox { get; set; }

    public string[] Words { get; set; }

    public string Text { get; set; }

    
}
