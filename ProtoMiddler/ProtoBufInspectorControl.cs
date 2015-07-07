using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ProtoMiddler
{
    public partial class ProtoBufInspectorControl : UserControl
    {
        public string MessageType;
        public string ProtoFile;

        public ProtoBufInspectorControl()
        {
            InitializeComponent();
            ProtoFile = string.Empty;
            MessageType = string.Empty;
        }

        public byte[] ProtobufBytes { get; set; }

        public string Data
        {
            get { return rtbData.Text; }

            set { rtbData.Text = value; }
        }

        public byte[] Encode()
        {
            if (string.IsNullOrEmpty(ProtoFile) || string.IsNullOrEmpty(MessageType))
            {
                return ProtobufBytes;
            }

            // try to encode using these things...

            return ProtoBufUtil.EncodeWithProto(Data, MessageType, ProtoFile);
        }

        void bnBrowse_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                txtProtoFile.Text = openFileDialog1.FileName;

                ProtoFile = txtProtoFile.Text.Trim();
                // also parse the proto file to fill in the cbType combo box
                if (File.Exists(ProtoFile))
                {
                    string rawProtoFile = File.ReadAllText(ProtoFile);
                    string[] tokens = rawProtoFile.Split(" \r\t\n{},".ToCharArray(),
                        StringSplitOptions.RemoveEmptyEntries);
                    var list = new List<string>();
                    for (int x = 0; x < tokens.Length; x++)
                    {
                        if (tokens[x].CompareTo("message") == 0)
                        {
                            string t = tokens[x + 1];
                            list.Add(t.Trim());
                        }
                    }
                    list.Sort();
                    cbType.Items.AddRange(list.ToArray());
                    cbType.Enabled = true;
                }
            }
        }

        void bnDecodeAs_Click(object sender, EventArgs e)
        {
            MessageType = (string) cbType.SelectedItem;
            ProtoFile = txtProtoFile.Text;

            Data = ProtoBufUtil.DecodeWithProto(ProtobufBytes, MessageType, ProtoFile);
        }
    }
}
