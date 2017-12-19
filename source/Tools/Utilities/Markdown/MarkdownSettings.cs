﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Utilities.Markdown
{
    //TODO: EmptyLineBetweenListItems
    public class MarkdownSettings
    {
        public MarkdownSettings(
            string boldDelimiter = "**",
            string italicDelimiter = "*",
            string strikethroughDelimiter = "~~",
            string listItemStart = "* ",
            string tableDelimiter = "|",
            string codeDelimiter = "`",
            string codeBlockChars = "```",
            string horizontalRule = "___",
            EmptyLineOptions headerOptions = EmptyLineOptions.EmptyLineBeforeAndAfter,
            EmptyLineOptions codeBlockOptions = EmptyLineOptions.EmptyLineBeforeAndAfter,
            bool useTablePadding = true,
            bool useTableOuterPipe = true,
            TableFormatting tableFormatting = TableFormatting.Header,
            string indentChars = "\t",
            MarkdownEscaper escaper = null)
        {
            BoldDelimiter = boldDelimiter;
            ItalicDelimiter = italicDelimiter;
            StrikethroughDelimiter = strikethroughDelimiter;
            ListItemStart = listItemStart;
            TableDelimiter = tableDelimiter;
            CodeDelimiter = codeDelimiter;
            CodeBlockChars = codeBlockChars;
            HorizontalRule = horizontalRule;
            HeaderOptions = headerOptions;
            CodeBlockOptions = codeBlockOptions;
            TableFormatting = tableFormatting;
            UseTablePadding = useTablePadding;
            UseTableOuterPipe = useTableOuterPipe;
            IndentChars = indentChars;
            Escaper = escaper ?? MarkdownEscaper.Default;
        }

        public static MarkdownSettings Default { get; } = new MarkdownSettings();

        public string BoldDelimiter { get; }

        public string AlternativeBoldDelimiter
        {
            get { return (BoldDelimiter == "**") ? "__" : "**"; }
        }

        public string ItalicDelimiter { get; }

        public string AlternativeItalicDelimiter
        {
            get { return (ItalicDelimiter == "*") ? "_" : "*"; }
        }

        public string StrikethroughDelimiter { get; }

        public string ListItemStart { get; }

        public string TableDelimiter { get; }

        public string CodeDelimiter { get; }

        public string CodeBlockChars { get; }

        public string HorizontalRule { get; }

        public EmptyLineOptions HeaderOptions { get; }

        internal bool AddEmptyLineBeforeHeader
        {
            get { return (HeaderOptions & EmptyLineOptions.EmptyLineBefore) != 0; }
        }

        internal bool AddEmptyLineAfterHeader
        {
            get { return (HeaderOptions & EmptyLineOptions.EmptyLineAfter) != 0; }
        }

        public EmptyLineOptions CodeBlockOptions { get; }

        internal bool AddEmptyLineBeforeCodeBlock
        {
            get { return (CodeBlockOptions & EmptyLineOptions.EmptyLineBefore) != 0; }
        }

        internal bool AddEmptyLineAfterCodeBlock
        {
            get { return (CodeBlockOptions & EmptyLineOptions.EmptyLineAfter) != 0; }
        }

        public TableFormatting TableFormatting { get; }

        internal bool FormatTableHeader
        {
            get { return (TableFormatting & TableFormatting.Header) != 0; }
        }

        internal bool FormatTableContent
        {
            get { return (TableFormatting & TableFormatting.Content) != 0; }
        }

        public bool UseTablePadding { get; }

        public bool UseTableOuterPipe { get; }

        public string IndentChars { get; }

        public MarkdownEscaper Escaper { get; }

        public string Escape(string value)
        {
            return Escaper.Escape(value);
        }

        internal string EscapeIf(bool condition, string value)
        {
            return (condition) ? Escaper.Escape(value) : value;
        }

        public bool ShouldBeEscaped(char value)
        {
            return Escaper.ShouldBeEscaped(value);
        }
    }
}
