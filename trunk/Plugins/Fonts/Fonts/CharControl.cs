using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Fonts
{
    public partial class CharControl : UserControl
    {
        Byte[] tiles;
        int depth;

        int charCode;
        CharView viewMode;
        sNFTR.HDWC.Info tileInfo;
        int width;
        int height;

        public CharControl()
        {
            InitializeComponent();
        }
        public CharControl(CharView mode, int charCode, sNFTR.HDWC.Info tileInfo, Byte[] tiles, int depth, int width, int height)
        {
            InitializeComponent();

            this.charCode = charCode;
            this.viewMode = mode;
            this.tileInfo = tileInfo;
            this.tiles = tiles;
            this.depth = depth;
            this.width = width;
            this.height = height;
        }

        private void Draw_Border(Color color)
        {
            
        }
        private void Draw_Char()
        {
            switch (viewMode)
            {
                case CharView.Edit:
                    break;
                case CharView.Font:
                    picFont.Image = NFTR.Get_Char(tiles, depth, width, height);
                    this.BackColor = Color.Transparent;
                    Border = Color.Transparent;
                    break;
                case CharView.Image:
                    break;
            }
        }
        public Color Border
        {
            set
            {
                picFont.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.BorderStyle = System.Windows.Forms.BorderStyle.None;
                
            }      
        }
        public int CharCode
        {
            get { return charCode; }
        }
        public CharView ViewMode
        {
            get { return viewMode; }
            set { viewMode = value; Draw_Char(); }
        }
        public int CharWidth
        {
            get { return width; }
        }
        public int CharHeight
        {
            get { return height; }
        }
        public int Depth
        {
            get { return depth; }
        }
        public sNFTR.HDWC.Info TileInfo
        {
            get { return tileInfo; }
        }
        public Byte[] Tiles
        {
            get { return tiles; }
        }
    }
    public enum CharView
    {
        Edit,
        Font,
        Image
    }
}
