﻿using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ProtoMiddler.ProtoGen;

namespace ProtoMiddler
{
    public partial class ProtoBufInspectorControl : UserControl
    {
        string MessageType;
        string ProtoFile;

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
                    var list = new ProtoLoader(ProtoFile).LoadTypes()
                        .OrderByName(true)
                        .Select(o => string.Join(".", o.Item1, o.Item2));
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
