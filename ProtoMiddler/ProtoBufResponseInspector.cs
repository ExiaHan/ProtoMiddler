/*This file is part of ProtoMiddler

ProtoMiddler is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Foobar is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Foobar. If not, see http://www.gnu.org/licenses/.
 
Author: Jon Boyd
Email: jboyd[at]securityinnovation[dot]com

 * */

﻿using System.Windows.Forms;
using Fiddler;

namespace ProtoMiddler
{
    public class ProtoBufResponseInspector : Inspector2, IResponseInspector2
    {
        //private RichTextBox _myControl;
        byte[] _entityBody;
        ProtoBufInspectorControl _myControl;
        bool m_bReadOnly;

        public void Clear()
        {
            _myControl.Data = string.Empty;
        }

        public byte[] body
        {
            get
            {
                if (_myControl.Text.CompareTo("NA") != 0)
                {
                    return _myControl.Encode();
                    //return ProtoBufUtil.Encode(_myControl.Data);
                    // return the protobuf encoded
                }
                return _entityBody;
            }
            set { _entityBody = value; }
        }

        public bool bDirty
        {
            get { return true; }
        }

        public bool bReadOnly
        {
            get { return true; }
            set { m_bReadOnly = value; }
        }


        public HTTPResponseHeaders headers { get; set; }

        public override void AssignSession(Session oSession)
        {
            byte[] protobufBytes = null;

            if (oSession.oResponse["Content-Type"].ToLower().Contains("protobuf"))
            {
                protobufBytes = oSession.responseBodyBytes;

                _entityBody = protobufBytes;
                _myControl.Data = ProtoBufUtil.DecodeRaw(protobufBytes);
                _myControl.ProtobufBytes = protobufBytes;
            }
            else
            {
                _myControl.Data = "NA"; // oSession.requestBodyBytes
            }
        }

        public override void AddToTab(TabPage o)
        {
            _entityBody = new byte[2048];
            _myControl = new ProtoBufInspectorControl();
            o.Text = "ProtoBuf";
            o.Controls.Add(_myControl);
            o.Controls[0].Dock = DockStyle.Fill;
        }

        public override int GetOrder()
        {
            return 0;
        }
    }
}
