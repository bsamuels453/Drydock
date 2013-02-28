using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Drydock.Render;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Drydock.Logic.DoodadEditorState {
    static class AirshipPackager {
        const int _version = 0;

        static public void Export(string fileName, HullDataManager hullData){
            JObject jObj = new JObject();
            jObj["Version"] = _version;
            jObj["NumDecks"] = hullData.NumDecks;

            var hullInds = new int[hullData.NumDecks][];
            var hullVerts = new VertexPositionNormalTexture[hullData.NumDecks][];

            for (int i = 0; i < hullData.NumDecks; i++){
                hullInds[i] = hullData.HullBuffers[i].DumpIndicies();
                hullVerts[i] = hullData.HullBuffers[i].DumpVerticies();
            }

            jObj["HullVerticies"] = JToken.FromObject(hullVerts);
            jObj["HullIndicies"] = JToken.FromObject(hullInds);

            var deckGeometry = new ObjectBuffer<ObjectIdentifier>.ObjectData[hullData.NumDecks][];

            for (int i = 0; i < hullData.NumDecks; i++){
                deckGeometry[i] = hullData.DeckBuffers[i].DumpObjectData();
            }

            jObj["DeckObjects"] = JToken.FromObject(deckGeometry);

            var sw = new StreamWriter(Directory.GetCurrentDirectory()+"\\Data\\"+fileName);
            sw.Write(JsonConvert.SerializeObject(jObj, Formatting.Indented));
            sw.Close();
        }
    }
}
