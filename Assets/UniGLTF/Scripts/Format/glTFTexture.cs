﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace UniGLTF
{
    [Serializable]
    public struct gltfImage
    {
        public string uri;
        public int bufferView;
    }

    public struct TextureWithIsAsset
    {
        public Texture2D Texture;
        public bool IsAsset;
    }

    public struct TextureWithImage
    {
        public gltfTexture Texture;
        public gltfTexture Image;
    }

    [Serializable]
    public struct gltfTexture
    {
        public int sampler;
        public int source;

        public TextureWithIsAsset GetTexture(string dir, glTF buffer, List<gltfImage> images)
        {
            var image = images[source];
            if (string.IsNullOrEmpty(image.uri))
            {
                // use buffer view
                var texture = new Texture2D(2, 2);
                //texture.name = string.Format("texture#{0:00}", i++);
                var byteSegment = buffer.GetViewBytes(image.bufferView);
                var bytes = byteSegment.Array.Skip(byteSegment.Offset).Take(byteSegment.Count).ToArray();
                texture.LoadImage(bytes, true);
                return new TextureWithIsAsset { Texture = texture, IsAsset = false };
            }
            else if (dir.StartsWith("Assets/"))
            {
                // local folder
                var path = Path.Combine(dir, image.uri);
                Debug.LogFormat("load texture: {0}", path);

                var texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                return new TextureWithIsAsset { Texture = texture, IsAsset = true };
            }
            else
            {
                // external
                var path = Path.Combine(dir, image.uri);
                var bytes = File.ReadAllBytes(path);

                var texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);
                return new TextureWithIsAsset { Texture = texture, IsAsset = true };
            }
        }
    }
}
