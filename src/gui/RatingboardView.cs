using DeepFlight.network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.view.gui {
    
    /// <summary>
    /// A board to show the highest player ratings in rows.
    /// </summary>
    class RatingboardView : View {

        private TextView
            text_Title,
            text_UserRankLabel,
            text_UserRankValue,
            text_UserRatingLabel,
            text_UserRatingValue;

        private BorderView border;
        private Row[] rows = new Row[5];

        public string Title {
            get => text_Title.Text;
            set => text_Title.Text = value;
        }

        public RatingboardView(Camera camera, string titleLabel, double x, double y, float width, float height) : base(camera) {
            X = x;
            Y = y;

            border = new BorderView(camera, borderWidth: 5f, x: x, y: y, width: width, height: height);
            border.BackgroundColor = new Color(Color.Black, 0.25f);
            AddChild(border);

            text_Title = new TextView(camera, titleLabel, size: 24, x: x, y: y - height * 0.60);
            AddChild(text_Title);

            text_UserRankLabel = new TextView(camera, "Your Rank:", size: 24, x: x-width * 0.25, y: y+height*0.62);
            text_UserRankValue = new TextView(camera, "- - -", size: 24, x: x-width * 0.25, y: y + height * 0.75);
            AddChildren(text_UserRankLabel, text_UserRankValue);

            text_UserRatingLabel = new TextView(camera, "Your Rating:", size: 24, x: x + width * 0.25, y: y + height * 0.62);
            text_UserRatingValue = new TextView(camera, "- - -", size: 24, x: x + width * 0.25, y: y + height * 0.75);
            AddChildren(text_UserRatingLabel, text_UserRatingValue);

            // Setup Header and Rows
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


        public void UpdateRankings(List<UserRanking> rankings) {
            for( int i=0; i < rankings.Count && i < 5; i++) {
                rows[i].cells[1].Text = rankings[i].name;
                rows[i].cells[2].Text = string.Format("{0:N2}", rankings[i].rating);
            }
        }

        public void UpdateUserRanking(UserRanking ranking) {
            text_UserRankValue.Text = "#" + ranking.rank;
            text_UserRatingValue.Text = string.Format("{0:N2}", ranking.rating);
        }

        public void HideUserRanking() {
            text_UserRatingValue.Hidden = true;
            text_UserRatingLabel.Hidden = true;
            text_UserRankValue.Hidden = true;
            text_UserRankLabel.Hidden = true;
        }


        public void HideRankings() {
            HideUserRanking();
            foreach( var row in rows) {
                row.Hidden = true;
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
