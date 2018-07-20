﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using AideDeJeu.Tools;
using Markdig.Syntax;

namespace AideDeJeuLib
{
    public class Generic : Item
    {
        public string Text { get; set; }

        public override string Markdown
        {
            get
            {
                return 
                    $"# {Name}\n\n" +
                    $"{AltName}\n\n" +
                    Text;
            }
        }

        public override void Parse(ref ContainerBlock.Enumerator enumerator)
        {
            enumerator.MoveNext();
            while (enumerator.Current != null)
            {
                var block = enumerator.Current;
                //DumpBlock(block);
                if (block is Markdig.Syntax.HeadingBlock)
                {
                    var headingBlock = block as Markdig.Syntax.HeadingBlock;
                    //DumpHeadingBlock(headingBlock);
                    if (headingBlock.HeaderChar == '#' && (headingBlock.Level == 1 || headingBlock.Level == 2))
                    {
                        if (this.Name == null)
                        {
                            this.Name = headingBlock.Inline.ToMarkdownString();
                        }
                        else
                        {
                            this.Text += new string(headingBlock.HeaderChar, headingBlock.Level) + " " + headingBlock.Inline.ToMarkdownString() + "\n\n";
                        }
                    }
                    else
                    {
                        this.Text += new string(headingBlock.HeaderChar, headingBlock.Level) + " " + headingBlock.Inline.ToMarkdownString() + "\n\n";
                    }
                }
                if (block is Markdig.Syntax.ParagraphBlock)
                {
                    if (block.IsNewItem())
                    {
                        return;
                    }
                    var paragraphBlock = block as Markdig.Syntax.ParagraphBlock;
                    this.Text += paragraphBlock.ToMarkdownString() + "\n";
                }
                if (block is Markdig.Syntax.ListBlock)
                {
                    var listBlock = block as Markdig.Syntax.ListBlock;
                    //DumpListBlock(listBlock);
                    if (listBlock.BulletType == '-')
                    {
                        foreach (var inblock in listBlock)
                        {
                            //DumpBlock(inblock);
                            var regex = new Regex("(?<key>.*?): (?<value>.*)");
                            if (inblock is Markdig.Syntax.ListItemBlock)
                            {
                                var listItemBlock = inblock as Markdig.Syntax.ListItemBlock;
                                foreach (var ininblock in listItemBlock)
                                {
                                    //DumpBlock(ininblock);
                                    if (ininblock is Markdig.Syntax.ParagraphBlock)
                                    {
                                        var paragraphBlock = ininblock as Markdig.Syntax.ParagraphBlock;
                                        //DumpParagraphBlock(paragraphBlock);
                                        var str = paragraphBlock.Inline.ToMarkdownString();

                                        var properties = new List<Tuple<string, Action<Generic, string>>>()
                                        {
                                            new Tuple<string, Action<Generic, string>>("AltName: ", (m, s) => m.AltName = s),
                                        };

                                        foreach (var property in properties)
                                        {
                                            if (str.StartsWith(property.Item1))
                                            {
                                                property.Item2.Invoke(this, str.Substring(property.Item1.Length));
                                                break;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        int index = 0;
                        foreach (var inblock in listBlock)
                        {
                            index++;
                            if (inblock is Markdig.Syntax.ListItemBlock)
                            {
                                var listItemBlock = inblock as Markdig.Syntax.ListItemBlock;
                                foreach (var ininblock in listItemBlock)
                                {
                                    //DumpBlock(ininblock);
                                    if (ininblock is Markdig.Syntax.ParagraphBlock)
                                    {
                                        var paragraphBlock = ininblock as Markdig.Syntax.ParagraphBlock;
                                        if (listBlock.IsOrdered)
                                        {
                                            this.Text += $"{index}. " + paragraphBlock.ToMarkdownString() + "";
                                        }
                                        else
                                        {
                                            this.Text += listBlock.BulletType + " " + paragraphBlock.ToMarkdownString() + "";
                                        }
                                    }
                                    else if (ininblock is ListBlock)
                                    {
                                        var listBlock2 = ininblock as ListBlock;
                                        foreach (var inblock2 in listBlock2)
                                        {
                                            if (inblock2 is Markdig.Syntax.ListItemBlock)
                                            {
                                                var listItemBlock2 = inblock2 as Markdig.Syntax.ListItemBlock;
                                                foreach (var ininblock2 in listItemBlock2)
                                                {
                                                    //DumpBlock(ininblock);
                                                    if (ininblock2 is Markdig.Syntax.ParagraphBlock)
                                                    {
                                                        var paragraphBlock2 = ininblock2 as Markdig.Syntax.ParagraphBlock;
                                                        this.Text += "  " + listBlock2.BulletType + " " + paragraphBlock2.ToMarkdownString() + "\n";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (block is Markdig.Extensions.Tables.Table)
                {
                    var tableBlock = block as Markdig.Extensions.Tables.Table;
                    this.Text += "\n\n" + tableBlock.ToMarkdownString() + "\n\n";
                }

                enumerator.MoveNext();

            }
        }
    }
}
