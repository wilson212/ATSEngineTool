using System;
using System.Collections.Generic;
using System.Text;

namespace ATSEngineTool
{
    /// <summary>
    /// This class is used to create formated Sii files.
    /// </summary>
    public sealed class SiiFileBuilder
    {
        /// <summary>
        /// The internal string builder
        /// </summary>
        private StringBuilder Builder { get; set; }

        /// <summary>
        /// Indicates whether the last character in the builder is a new line
        /// </summary>
        private bool NewLine = true;

        /// <summary>
        /// Gets or sets a value indicating whether to add an indent to structs within 
        /// the SiiNunit wrapper
        /// </summary>
        public bool IndentStructs { get; set; } = true;

        /// <summary>
        /// Gets or sets the character string to use when indenting. This can be set to any string value. 
        /// However, to ensure valid SII format, you should specify only valid white space 
        /// characters, such as space characters, tabs, carriage returns, or line feeds. 
        /// The default is a tab.
        /// </summary>
        public string IndentChars { get; set; } = "\t";

        /// <summary>
        /// Gets or Sets the current indent level of the document
        /// </summary>
        public int Indent
        {
            get { return _indent; }
            set
            {
                if (value < 0)
                    value = 0;

                _indent = value;
            }
        }

        private int _indent = 0;

        /// <summary>
        /// Indicates whether a struct is currently open
        /// </summary>
        public bool OpenStruct { get; private set; } = false;

        /// <summary>
        /// Indicates whether the SiiNunit document block is open
        /// </summary>
        public bool DocumentOpen { get; private set; } = false;

        /// <summary>
        /// Creates a new instance of <see cref="SiiFileBuilder"/>
        /// </summary>
        public SiiFileBuilder()
        {
            Builder = new StringBuilder();
        }

        /// <summary>
        /// Opens the SiiNUnit tag, and prepares the document for structs
        /// </summary>
        public void WriteStartDocument()
        {
            // Reset
            Builder.Clear();
            OpenStruct = false;
            Indent = 0;

            // Write document start
            Builder.AppendLine("SiiNunit");
            Builder.AppendLine("{");

            // Apply first indent
            if (IndentStructs)
                Indent++;

            // Set internals
            DocumentOpen = true;
        }

        /// <summary>
        /// Closes all open structs as well as the document
        /// </summary>
        public void WriteEndDocument()
        {
            if (!DocumentOpen)
                throw new Exception("The document is not open");

            // Close open struct
            if (OpenStruct)
                WriteStructEnd();

            // Reset indent and close document
            Indent = 0;
            Builder.Append("}");

            // Set internals
            DocumentOpen = false;
        }

        /// <summary>
        /// Writes a multi line comment block
        /// </summary>
        /// <param name="lines">The lines to be added to the comment</param>
        /// <returns></returns>
        public SiiFileBuilder WriteCommentBlock(IEnumerable<string> lines)
        {
            // Open comment
            this.WriteLine("/**");

            // Write each line
            foreach (string line in lines)
                this.WriteLine($" * {line}");

            // Close comment
            this.WriteLine(" */");
            return this;
        }

        /// <summary>
        /// Opens a new Struct in the document
        /// </summary>
        /// <param name="type">The struct type name</param>
        /// <param name="name">The name of this struct instance</param>
        /// <returns></returns>
        public SiiFileBuilder WriteStructStart(string type, string name)
        {
            // Ensure we are exclusive
            if (OpenStruct)
                throw new Exception("Cannot open more than 1 struct at a time");

            // Ensure the document is open
            if (!DocumentOpen)
                throw new Exception("Cannot write a struct outside of the document");

            // Write the type and name line
            Builder.AppendIf(NewLine, IndentChars, Indent)
                .Append(type)
                .Append(" : ")
                .AppendLine(name);

            // Add Open brace   
            Builder.Append(IndentChars, Indent).AppendLine("{");

            // Inicate new line and increase indent
            NewLine = true;
            Indent++;
            OpenStruct = true;
            return this;
        }

        /// <summary>
        /// Closes the current open struct
        /// </summary>
        /// <returns></returns>
        public SiiFileBuilder WriteStructEnd()
        {
            // Always end on a new line!
            if (!NewLine) this.WriteLine();

            // Ensure that all structs are not closed
            if (!OpenStruct)
                throw new Exception("There are no open structs!");

            // Decrease indent first to align with open struct tag
            Indent--;

            // Write close brace and decrease indent
            Builder.Append(IndentChars, Indent).AppendLine("}");
            NewLine = true;
            OpenStruct = false;
            return this;
        }

        /// <summary>
        /// Writes an attribute and its value to the document string buffer.
        /// </summary>
        /// <param name="name">The Attribute Name</param>
        /// <param name="value">The string value of the attribute</param>
        /// <param name="quote">Indicates whether the value needs to be wrapped in quotes</param>
        /// <param name="comment">The in line comment for this attribute</param>
        /// <param name="commentPadding">Specifies the number of indents that are applied before the comment is written</param>
        public SiiFileBuilder WriteAttribute(string name, string value, bool quote = true, string comment = "", int commentPadding = 1)
        {
            // Make sure there is a struct open
            if (!OpenStruct)
                throw new Exception("Cannot write attribute when there are no open structs!");

            // Always start on a newline!
            if (!NewLine) this.WriteLine();

            // Append the attribute string
            Builder.Append(IndentChars, Indent).Append(name).Append(':');
            Builder.Append(' ').AppendIf(quote, '"').Append(value).AppendIf(quote, '"');

            // Append comment or close line
            if (!String.IsNullOrEmpty(comment))
            {
                Builder.Append('\t', commentPadding);
                Builder.Append("# ").AppendLine(comment);
            }
            else
            {
                Builder.AppendLine();
            }

            NewLine = true;
            return this;
        }

        /// <summary>
        /// Writes an attribute and its value to the document string buffer.
        /// </summary>
        /// <param name="name">The Attribute Name</param>
        /// <param name="value">The integer value of the attribute</param>
        /// <param name="comment">The in line comment for this attribute</param>
        /// <param name="commentPadding">Specifies the number of indents that are applied before the comment is written</param>
        public SiiFileBuilder WriteAttribute(string name, int value, string comment = "", int commentPadding = 1)
        {
            return WriteAttribute(name, value.ToString(), false, comment, commentPadding);
        }

        /// <summary>
        /// Writes an attribute and its value to the document string buffer.
        /// </summary>
        /// <param name="name">The Attribute Name</param>
        /// <param name="value">The decimal value of the attribute</param>
        /// <param name="comment">The in line comment for this attribute</param>
        /// <param name="commentPadding">Specifies the number of indents that are applied before the comment is written</param>
        public SiiFileBuilder WriteAttribute(string name, decimal value, string comment = "", int commentPadding = 1)
        {
            return WriteAttribute(name, value.ToString(Program.NumberFormat), false, comment, commentPadding);
        }

        /// <summary>
        /// Writes an attribute and its value to the document string buffer.
        /// </summary>
        /// <param name="name">The Attribute Name</param>
        /// <param name="value">The bool value of the attribute</param>
        /// <param name="comment">The in line comment for this attribute</param>
        /// <param name="commentPadding">Specifies the number of indents that are applied before the comment is written</param>
        public SiiFileBuilder WriteAttribute(string name, bool value, string comment = "", int commentPadding = 1)
        {
            return WriteAttribute(name, value ? "true" : "false", false, comment, commentPadding);
        }

        /// <summary>
        /// Writes an attribute and its value to the document string buffer.
        /// </summary>
        /// <param name="name">The Attribute Name</param>
        /// <param name="value">The double value of the attribute</param>
        /// <param name="comment">The in line comment for this attribute</param>
        /// <param name="commentPadding">Specifies the number of indents that are applied before the comment is written</param>
        public SiiFileBuilder WriteAttribute(string name, double value, string comment = "", int commentPadding = 1)
        {
            return WriteAttribute(name, value.ToString(Program.NumberFormat), false, comment, commentPadding);
        }

        /// <summary>
        /// Clears the current contents of this StringBuilder
        /// </summary>
        /// <param name="builder"></param>
        public void Clear()
        {
            Builder.Clear();
            NewLine = true;
        }

        /// <summary>
        /// Appends a directive to the current buffer
        /// </summary>
        public SiiFileBuilder WriteDirective(string value)
        {
            // Always write on a new line!
            if (!NewLine) this.WriteLine();

            // Just append, no tabs
            Builder.AppendLine(value);
            NewLine = true;
            return this;
        }

        /// <summary>
        /// Appends a new line
        /// </summary>
        public SiiFileBuilder WriteLine()
        {
            Builder.AppendLine();
            NewLine = true;
            return this;
        }

        /// <summary>
        /// Appends an Object to this string builder if the <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="condition">Indicates whether we append this object to the end of this StringBuilder</param>
        /// <param name="value">The value to append</param>
        public SiiFileBuilder WriteIf(bool condition, object value)
        {
            if (condition)
                Builder.AppendIf(NewLine, IndentChars, Indent).Append(value);

            NewLine = false;
            return this;
        }

        /// <summary>
        /// Appends a string to this string builder if the <paramref name="condition"/> is true, followed by a line terminator.
        /// </summary>
        /// <param name="condition">Indicates whether we append this object to the end of this StringBuilder</param>
        /// <param name="value">The value to append</param>
        public SiiFileBuilder WriteLineIf(bool condition, string value)
        {
            if (condition)
            {
                Builder.AppendIf(NewLine, IndentChars, Indent).AppendLine(value);
                NewLine = true;
            }
            return this;
        }

        /// <summary>
        /// Appends a string to this string builder if the <paramref name="condition"/> is true, followed by a line terminator.
        /// </summary>
        /// <param name="condition">Indicates whether we append this object to the end of this StringBuilder</param>
        /// <param name="value">The value to append</param>
        public SiiFileBuilder WriteLineIf(bool condition, string trueValue, string falseValue)
        {
            if (condition)
                Builder.AppendIf(NewLine, IndentChars, Indent).AppendLine(trueValue);
            else
                Builder.AppendIf(NewLine, IndentChars, Indent).AppendLine(falseValue);

            NewLine = true;
            return this;
        }

        /// <summary>
        /// Appends a new line to this string builder if the <paramref name="condition"/> is true
        /// </summary>
        /// <param name="condition">Indicates whether we append a newline to the end of this StringBuilder</param>
        public SiiFileBuilder WriteLineIf(bool condition)
        {
            if (condition)
            {
                Builder.AppendIf(NewLine, IndentChars, Indent).AppendLine();
                NewLine = true;
            }
            return this;
        }

        /// <summary>
        /// Appends the specified string to the document string buffer
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SiiFileBuilder Write(string value)
        {
            Builder.AppendIf(NewLine, IndentChars, Indent).Append(value);
            NewLine = false;
            return this;
        }

        /// <summary>
        /// Appends the specified string to the document string buffer, followed by a new line
        /// </summary>
        public SiiFileBuilder WriteLine(string value)
        {
            Builder.AppendIf(NewLine, IndentChars, Indent).AppendLine(value);
            NewLine = true;
            return this;
        }

        public override string ToString() => Builder.ToString();
    }
}
