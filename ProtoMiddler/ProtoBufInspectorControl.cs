using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ProtoMiddler.ProtoGen;

namespace ProtoMiddler
{
    public partial class ProtoBufInspectorControl : UserControl
    {
        readonly Dictionary<string, string> _lastUsedTypes = new Dictionary<string, string>();
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
                UpdateTypeList();
            }
        }

        /// <summary>
        ///     parse the proto file to fill in the cbType combo box
        /// </summary>
        void UpdateTypeList()
        {
            cbType.Enabled = false;
            cbType.SelectedItem = null;
            cbType.Items.Clear();
            if (File.Exists(ProtoFile))
            {
                var list = new ProtoLoader(ProtoFile).LoadTypes()
                    .OrderByName(true);
                cbType.Items.AddRange(list.ToArray());
                cbType.Enabled = true;
                SetLastUsedType();
            }
        }

        void bnDecodeAs_Click(object sender, EventArgs e)
        {
            MessageType = (string) cbType.SelectedItem;
            ProtoFile = txtProtoFile.Text;

            if (!string.IsNullOrWhiteSpace(ProtoFile) &&
                !string.IsNullOrWhiteSpace(MessageType))
            {
                Data = ProtoBufUtil.DecodeWithProto(ProtobufBytes, MessageType, ProtoFile);
            }
        }

        void cbType_OnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            var file = txtProtoFile.Text;
            var type = (string) cbType.SelectedItem;
            if (file != null)
            {
                var key = Path.GetFileName(file).Trim();
                if (!string.IsNullOrEmpty(key) &&
                    !string.IsNullOrWhiteSpace(type))
                {
                    _lastUsedTypes[key] = type.Trim();
                }
            }
        }

        void SetLastUsedType()
        {
            var file = txtProtoFile.Text;
            if (file != null)
            {
                var key = Path.GetFileName(file).Trim();
                if (_lastUsedTypes.ContainsKey(key))
                {
                    var value = _lastUsedTypes[key];
                    if (cbType.Items.Contains(value))
                    {
                        cbType.SelectedItem = value;
                    }
                }
            }
        }
    }
}
