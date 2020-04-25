using DeepFlight.network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.view.gui {
    
    /// <summary>
    /// A board to show the highest player ratings
    /// </summary>
    class RatingboardView : View {

        private TextView title;
        private BorderView border;
        private Row[] rows = new Row[5];

        public RatingboardView(Camera camera, string titleLabel, double x, double y, float width, float height) : base(camera) {

            border = new BorderView(camera, borderWidth: 5f, x: x, y: y, width: width, height: height);
            border.BackgroundColor = new Color(Color.Black, 0.25f);
            AddChild(border);

            title = new TextView(camera, titleLabel, size: 24, x: x, y: y - height * 0.60);
            AddChild(title);

            var margin = 0.07f;// Top bottom margin
            var rowSpacing = (height * (1-margin*2)) / 5f;
            var rowY = y - height * (0.5-margin);

            var header = new Row(camera, 14, new string[] { "Rank", "Username", "Rating" }, x, rowY, width);
            AddChild(header); 

            for( int i=0; i<5; i++) {
                rowY += rowSpacing;
                rows[i] = new Row(camera, 20, new string[] { "#"+(i+1), "- - - - -", "- - -" }, x, rowY, width);
                AddChild(rows[i]);
            }
        }


        public void UpdateRatings(List<UserRating> ratings) {
            for( int i=0; i < ratings.Count && i < 5; i++) {
                rows[i].cells[1].Text = ratings[i].name;
                rows[i].cells[2].Text = string.Format("{0:N2}", ratings[i].rating);
            }
        }



        private class Row : View {

            public TextView[] cells = new TextView[3];

            public Row( Camera camera, double fontSize, string[] initialText, double x, double y, float width ) {
                
                for( int i=0; i<3; i++) {
                    cells[i] = new TextView(camera, initialText[i], Font.DEFAULT, fontSize, Color.White, 0, y);
                    AddChild(cells[i]);
                }
                var leftX = x - width * 0.5;
                cells[0].X = leftX + width * 0.10;
                cells[1].X = leftX + width * 0.50;
                cells[2].X = leftX + width * 0.90;
            }

        }
    }
}
