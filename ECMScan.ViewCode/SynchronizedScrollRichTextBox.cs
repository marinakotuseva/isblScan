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
        public int[] WM_EVENTS = { 0x115, 0x20A};
		public int[] WM_NO_EVENTS = { 0x20, 0x200};
		
		private bool isScrollEvent(int eventCode)
		{
			foreach(int wm_eventCode in WM_EVENTS)
			{
				if(wm_eventCode == eventCode)
				{
					return true;
				}
			}
			return false;
		}
		
		private bool isNoScrollEvent(int eventCode)
		{
			foreach(int wm_eventCode in WM_NO_EVENTS)
			{
				if(wm_eventCode == eventCode)
				{
					return false;
				}
			}
			return true;
		}

		
		private int eventNumber = 0;
        List<SynchronizedScrollRichTextBox> peers = new List<SynchronizedScrollRichTextBox>();

        public void AddPeer(SynchronizedScrollRichTextBox peer)
        {
            this.peers.Add(peer);
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
