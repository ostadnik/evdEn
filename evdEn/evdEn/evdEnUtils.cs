using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using evdEnData;


namespace evdEn
{
    public class evdEnUtils
    {
        public static string SplitTextIntoLines(string text,
            int maxCharsPerLine, int maxLines)
        {
            if (string.IsNullOrEmpty(text)) return "";
            if (text.Length < maxCharsPerLine)
            {
                return text;
            }

            StringBuilder stringBuilder = new StringBuilder(text);
            int currentLine = 0;
            int newLineIndex = 0;
            while (((text.Length - newLineIndex) > maxCharsPerLine)
                && (currentLine < maxLines))
            {
                text.IndexOf(' ', 0);
                int nextIndex = newLineIndex;
                while ((nextIndex >= 0) && (nextIndex < maxCharsPerLine))
                {
                    newLineIndex = nextIndex;
                    nextIndex = text.IndexOf(' ', newLineIndex + 1);
                }
                stringBuilder.Replace(' ', '\n', newLineIndex, 1);
                currentLine++;
            }

            return stringBuilder.ToString();
        }

        public static string SplitTextIntoLines(string text,
            int maxCharsPerLine)
        {
            if (string.IsNullOrEmpty(text)) return "";
            if (text.Length < maxCharsPerLine)
            {
                return text;
            }

            StringBuilder stringBuilder = new StringBuilder(text);
            int currentLine = 0;
            int newLineIndex = 0;
            while (((text.Length - newLineIndex) > maxCharsPerLine))
            {
                text.IndexOf(' ', 0);
                int nextIndex = newLineIndex;
                while ((nextIndex >= 0) && (nextIndex < maxCharsPerLine))
                {
                    newLineIndex = nextIndex;
                    nextIndex = text.IndexOf(' ', newLineIndex + 1);
                }
                stringBuilder.Replace(' ', '\n', newLineIndex, 1);
                currentLine++;
            }

            return stringBuilder.ToString();
        }

        public static List<string> SplitTextIntoList(string text, SpriteFont font,
            int rowWidthInPix)
        {
            List<string> lines = new List<string>();

            if (string.IsNullOrEmpty(text))
            {
                lines.Add(string.Empty);
                return lines;
            }

            if (font.MeasureString(text).X <= rowWidthInPix)
            {
                lines.Add(text);
                return lines;
            }

            string[] words = text.Split(null);

            int currentWord = 0;
            while (currentWord < words.Length)
            {
                int wordsThisLine = 0;
                string line = String.Empty;
                while (currentWord < words.Length)
                {
                    string testLine = line;
                    if (testLine.Length < 1)
                    {
                        testLine += words[currentWord];
                    }
                    else if ((testLine[testLine.Length - 1] == '.')
                            || (testLine[testLine.Length - 1] == '?')
                            || (testLine[testLine.Length - 1] == '!'))
                    {
                        testLine += "  " + words[currentWord];
                    }
                    else
                    {
                        testLine += " " + words[currentWord];
                    }
                    if ((wordsThisLine > 0)
                        && (font.MeasureString(testLine).X > rowWidthInPix))
                    {
                        break;
                    }
                    line = testLine;
                    wordsThisLine++;
                    currentWord++;
                }
                lines.Add(line);
            }
            return lines;
        }

        public static int DrawTextInRect(string text, Rectangle dest,
            SpriteFont font, Color color, ref SpriteBatch spriteBatch)
        {
            return DrawTextInRect(text, dest, font, color, ref spriteBatch, 0);
        }

        public static int  DrawTextInRect(string text, Rectangle dest,
            SpriteFont font, Color color, ref SpriteBatch spriteBatch, int startLine)
        {
            if (string.IsNullOrEmpty(text)) return 0;

            List<string> list = evdEnUtils.SplitTextIntoList(text, font, dest.Width);
            return DrawTextInRect(list, dest, font, color, ref spriteBatch, startLine);
        }

        public static int DrawTextInRect(List<string> list, Rectangle dest,
            SpriteFont font, Color color, ref SpriteBatch spriteBatch)
        {
            return DrawTextInRect(list, dest, font, color, ref spriteBatch, 0);
        }

        public static int DrawTextInRect(List<string> list, Rectangle dest,
            SpriteFont font, Color color, ref SpriteBatch spriteBatch, int startLine)
        {
            if (list.Count <= 0) return 0;
            float y = 0;
            float dy = font.MeasureString("WY|ydkjQ^").Y;
            int i = startLine;
            while ((i < list.Count) && (y < dest.Height))
            {
                spriteBatch.DrawString(font, list[i], new Vector2(dest.X, dest.Y + y), color);
                i++;
                y += dy;
            }
            return i - startLine;
        }

        public static void DrawCenteredText(string text, Vector2 position,
            SpriteFont font, Color color, ref SpriteBatch spriteBatch)
        {
            if (string.IsNullOrEmpty(text)) return;

            Vector2 textSize = font.MeasureString(text);
            Vector2 centeredPosition = new Vector2(
                position.X - (int)textSize.X / 2,
                position.Y - (int)textSize.Y / 2);

            spriteBatch.DrawString(font, text, centeredPosition, color, 0f,
                Vector2.Zero, 1f, SpriteEffects.None, 1f - position.Y / 720f);
        }
    }
}
