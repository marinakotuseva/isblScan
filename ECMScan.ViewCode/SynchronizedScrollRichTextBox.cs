using System;
using System.Collections.Generic;
using System.Windows.Forms;
/*
 * Use code from
 * https://gist.github.com/593809
 * bhttp://stackoverflow.com/questions/3322741/synchronizing-multiline-textbox-positions-in-c-sharp
 */
namespace ISBLScan.ViewCode
{
	public class SynchronizedScrollRichTextBox : System.Windows.Forms.RichTextBox
	{
		public SynchronizedScrollRichTextBox()
		{
			this.DoubleBuffered = true;
		}
        public int[] WmEvents = { 0x115, 0x20A};
		public int[] WmNoEvents = { 0x20, 0x200};
		
		private bool IsScrollEvent(int eventCode)
		{
			foreach(int wmEventCode in WmEvents)
			{
				if(wmEventCode == eventCode)
				{
					return true;
				}
			}
			return false;
		}
		
		private bool IsNoScrollEvent(int eventCode)
		{
			foreach(int wmEventCode in WmNoEvents)
			{
				if(wmEventCode == eventCode)
				{
					return false;
				}
			}
			return true;
		}

		
		private int _eventNumber = 0;
        List<SynchronizedScrollRichTextBox> _peers = new List<SynchronizedScrollRichTextBox>();

        public void AddPeer(SynchronizedScrollRichTextBox peer)
        {
            this._peers.Add(peer);
        }

        public void DirectWndProc(ref Message m)
        {
            base.WndProc(ref m);
        }

        protected override void WndProc(ref Message m)
        {
			base.WndProc(ref m);
			//System.Console.WriteLine(System.String.Format("{4}\tHWND:{3:X}, MSG:{2:X}, W:{0:X}, H:{1:X}", m.WParam, m.LParam, m.Msg, m.HWnd, eventNumber.ToString("00000")));
			//eventNumber++;
			/*
            if (isScrollEvent(m.Msg))
            {
                foreach (var peer in this.peers)
                {
                    var peerMessage = Message.Create(peer.Handle, m.Msg, m.WParam, m.LParam);
                    peer.DirectWndProc(ref peerMessage);
                }
            }
            */
        }
	}
}
