using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using first_project.Models;
using System.Xml.Linq;


namespace first_project.Services
{
    class TileService
    {
        public static Windows.Data.Xml.Dom.XmlDocument CreateTiles(PrimaryTile primaryTile)
        {
            XDocument xDoc = new XDocument(
                new XElement("tile", new XAttribute("version", 3),
                    new XElement("visual",
                        // Small Tile
                        new XElement("binding", new XAttribute("branding", primaryTile.branding), new XAttribute("displayName", primaryTile.appName), new XAttribute("template", "TileSmall"),
                        new XElement("image", new XAttribute("placement", "background"), new XAttribute("src", "Assets/1.scale-200.png")),
                        new XElement("group",
                                new XElement("subgroup",
                                    new XElement("text", primaryTile.time, new XAttribute("hint-style", "caption")),
                                    new XElement("text", primaryTile.message, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                )
                            )
                        ),
                        // Wide Tile
                        new XElement("binding", new XAttribute("branding", primaryTile.branding), new XAttribute("displayName", primaryTile.appName), new XAttribute("template", "TileWide"),
                        new XElement("image", new XAttribute("placement", "background"), new XAttribute("src", "Assets/3.png")),
                        new XElement("group",
                                new XElement("subgroup",
                                    new XElement("text", primaryTile.time, new XAttribute("hint-style", "caption")),
                                    new XElement("text", primaryTile.message, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                    ),
                                new XElement("subgroup", new XAttribute("hint-weight", 15),
                                    new XElement("image", new XAttribute("placement", "inline"), new XAttribute("src", "Assets/StoreLogo.png"))
                                )
                            )
                        ),

                        // Medium Tile
                        new XElement("binding", new XAttribute("branding", primaryTile.branding), new XAttribute("displayName", primaryTile.appName), new XAttribute("template", "TileMedium"),
                            new XElement("image", new XAttribute("placement", "background"), new XAttribute("src", "Assets/2.png")),
                            new XElement("group",
                                new XElement("subgroup",
                                    new XElement("text", primaryTile.time, new XAttribute("hint-style", "caption")),
                                    new XElement("text", primaryTile.message, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                )
                            )
                        )
                    )
                )
            );

            Windows.Data.Xml.Dom.XmlDocument xmlDoc = new Windows.Data.Xml.Dom.XmlDocument();
            xmlDoc.LoadXml(xDoc.ToString());
            return xmlDoc;
        }



    }
}
