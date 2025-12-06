using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RatKing.Bloxels {

	// TODO put textures into another settings asset, so different levels can share different texture atlases

	[CreateAssetMenu(fileName = "BloxelProjectSettings", menuName = "Bloxels/BloxelProjectSettings")]
	[DefaultExecutionOrder(-10001)]
	public class BloxelProjectSettings : ScriptableObject {

		class TemporaryAtlasInfo {
			public int idx = -1;
			public List<List<Texture2D>> textures = new List<List<Texture2D>>();
			public int indices = 0;
			public bool mainHasTransparency = false;
		}

		public const string ASSETS_PATH_RESOURCES = "Assets/RatKing/Bloxels/Resources/";
		public const int TEXTURE_PADDING = 6;

		[SerializeField] TextureAtlasSettings[] texAtlases = null;
#if UNITY_EDITOR
		public bool ShowInternals => EditorPrefs.GetBool("BLOED_SHOW_INTERNALS");
#endif
		public TextureAtlasSettings[] TexAtlases => texAtlases;
		// textures
		[HideInInspector][SerializeField] BloxelTexture missingTexture;
		public BloxelTexture MissingTexture => missingTexture;
		// templates
		[HideInInspector][SerializeField] BloxelTemplateAir airTemplate;
		public BloxelTemplateAir AirTemplate => airTemplate;
		[HideInInspector][SerializeField] BloxelTemplate boxTemplate;
		public BloxelTemplate BoxTemplate => boxTemplate;
		[HideInInspector][SerializeField] BloxelTemplate missingTemplate;
		public BloxelTemplate MissingTemplate => missingTemplate;
		[HideInInspector][SerializeField] List<BloxelTemplate> templates;
		//[SerializeField] BloxelTemplateTransformDictionary hiddenBloxelMeshContainers; // TODO needed?
		[HideInInspector] public List<string> TypeShelves = new List<string>(); // TODO property
		[HideInInspector] public List<string> TextureShelves = new List<string>(); // TODO property

		// temporary:
		BloxelType[] types = null;
		public BloxelType[] Types => types ?? (types = Resources.LoadAll<BloxelType>("BloxelTypes"));

		//[HideInInspector] [SerializeField] List<BloxelTexture> textures = new List<BloxelTexture>();
		List<BloxelTexture> textures = null;
		public List<BloxelTexture> Textures {
			get {
				//var count = 0;
				//foreach (var ta in texAtlases) { if (ta != null) { count += ta.Textures.Count; } } // TODO optimize
				if (textures != null && textures.Count > 0) { return textures; } // != count) {
				textures = new List<BloxelTexture>();
				foreach (var ta in texAtlases) {
					if (ta == null) { continue; }
					foreach (var t in ta.Textures) {
						if (t == null || t.ID == null) { continue; }
						textures.Add(t);
					}
				}
				return textures;
			}
		}

		// ONLY USED DURING SERIALIZATION:
		Dictionary<string, BloxelTemplate> templatesByUID = null; // dictionaries can't get serialized
		public Dictionary<string, BloxelTemplate> TemplatesByUID {
			get {
				if (templates == null || templates.Count == 0) { return null; }
				if (templatesByUID == null || templatesByUID.Count != templates.Count) {
					templatesByUID = new Dictionary<string, BloxelTemplate>();
					foreach (var t in templates) {
						if (t == null || t.UID == null) { return null; }
						if (templatesByUID.ContainsKey(t.UID)) { Debug.LogWarning("Template ID " + t.ID + " is not unique! Please fix that!", t); }
						else { templatesByUID[t.UID] = t; }
					}
				}
				return templatesByUID;
			}
		}
		Dictionary<string, BloxelTexture> texturesByID = null; // dictionaries don't get serialized
		Dictionary<string, int> textureIndicesByID = null; // dictionaries don't get serialized
		public void ResetTextureLists() {
			texturesByID = null;
			textureIndicesByID = null;
		}
		public Dictionary<string, BloxelTexture> TexturesByID {
			get {
				var texes = Textures;
				if (texes == null || texes.Count == 0) { return null; }
				if (texturesByID == null || texturesByID.Count != texes.Count) {
					texturesByID = new Dictionary<string, BloxelTexture>();
					foreach (var t in texes) {
						if (texturesByID.ContainsKey(t.ID)) { Debug.LogWarning("Texture ID " + t.ID + " is not unique! Please fix that!", t); }
						else { texturesByID[t.ID] = t; }
					}
				}
				return texturesByID;
			}
		}
		public Dictionary<string, int> TextureIndicesByID {
			get {
				var texes = Textures;
				if (texes == null || texes.Count == 0) { return null; }
				if (textureIndicesByID == null || textureIndicesByID.Count != texes.Count) {
					textureIndicesByID = new Dictionary<string, int>();
					int i = 0;
					foreach (var t in texes) {
						if (textureIndicesByID.ContainsKey(t.ID)) { Debug.LogWarning("Texture ID " + t.ID + " is not unique! Please fix that!", t); }
						else { textureIndicesByID[t.ID] = i; }
						++i;
					}
				}
				return textureIndicesByID;
			}
		}

		//

		void OnEnable() {
			types = null;
		}

		T CreateTemplate<T>(BloxelType type, int dir, int rot) where T : BloxelTemplate {
			var vUID = type.ID + "_" + dir + "_" + rot;
			var vtemp = ScriptableObject.CreateInstance<T>();
			vtemp.Init(vUID, type, dir, rot);
			templates.Add(vtemp);
#if UNITY_EDITOR
			EditorUtility.SetDirty(type);
#endif
			return vtemp;
		}

		T CreateTemplate<T>(string UID, Mesh mesh = null) where T : BloxelTemplate {
			var vtemp = ScriptableObject.CreateInstance<T>();
			vtemp.Init(UID, mesh);
			templates.Add(vtemp);
			return vtemp;
		}

		void SaveTemplates() {
#if UNITY_EDITOR
			foreach (var t in templates) {
				if (t.Type == null) {
					var path = ASSETS_PATH_RESOURCES + "Generated/BloxelTemplates/" + t.name + ".asset";
					AssetDatabase.CreateAsset(t, path);
					AssetDatabase.ImportAsset(path);
				}
				else {
					var path = AssetDatabase.GetAssetPath(t.Type);
					AssetDatabase.AddObjectToAsset(t, path);
					AssetDatabase.ImportAsset(path);
					EditorUtility.SetDirty(t.Type);
				}
			}
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
#endif
		}

		public static void SaveTexture(BloxelTexture bt, string name) {
#if UNITY_EDITOR
			var path = ASSETS_PATH_RESOURCES + "Generated/BloxelTextures/" + name + ".asset";
			AssetDatabase.CreateAsset(bt, path);
			AssetDatabase.AddObjectToAsset(bt.texture, bt);
			EditorUtility.SetDirty(bt);
			AssetDatabase.ImportAsset(path);
			AssetDatabase.SaveAssets();
#endif
		}

		/// <summary>
		/// Prepare all the provided templates and do geometry stuff
		/// </summary>
		public void PrepareTemplates() {
			types = null;
			templatesByUID = null;
			var dateTime = System.DateTime.Now;

#if UNITY_EDITOR
			// A) clean
			if (!AssetDatabase.IsValidFolder(ASSETS_PATH_RESOURCES + "/Generated/BloxelTemplates")) {
				AssetDatabase.CreateFolder(ASSETS_PATH_RESOURCES + "/Generated", "BloxelTemplates");
			}
			foreach (var res in Resources.LoadAll<BloxelTemplate>("Generated/BloxelTemplates")) {
				AssetDatabase.DeleteAsset(ASSETS_PATH_RESOURCES + "Generated/BloxelTemplates/" + res.name + ".asset");
			}
#endif
			TypeShelves.Clear();
			foreach (var type in Types) {
				foreach (var t in type.Templates) {
					if (t == null) { continue; }
#if UNITY_EDITOR
					AssetDatabase.RemoveObjectFromAsset(t);
#endif
					DestroyImmediate(t);
				}
				type.Templates.Clear();

				if (!TypeShelves.Contains(type.Shelf)) { TypeShelves.Add(type.Shelf); }
			}

			// B) generate the generated ones

			if (templates == null) { templates = new List<BloxelTemplate>(); } else { templates.Clear(); }
			missingTemplate = CreateTemplate<BloxelTemplateAir>("$MISSING"); // first air, "null"
			airTemplate = CreateTemplate<BloxelTemplateAir>("AIR"); // second air, real

			var boxGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
			var boxMesh = boxGO.GetComponent<MeshFilter>().sharedMesh;
			GameObject.DestroyImmediate(boxGO);
			boxTemplate = CreateTemplate<BloxelTemplate>("CUBE", boxMesh); // this is the standard cube

			// C) generate the type'd templates

			foreach (var vt in Types) {
				for (int d = 0, b = 0; d < 6; ++d) { // directions
					for (int r = 0; r < 4; ++r, ++b) { // rotations
						if (vt.IsRotationPossible(b)) {
							CreateTemplate<BloxelTemplate>(vt, d, r);
						}
					}
				}
			}

			// assign collider templates
			foreach (var t in templates) {
				if (t.Type != null && t.Type.colliderType != null) {
					var ct = templates.Find(v => v.ID == t.Type.colliderType.ID && v.Dir == t.Dir && v.Rot == t.Rot);
					if (ct != null) { t.SetColliderTemplate(ct); }
					else { UnityEngine.Debug.LogWarning("Couldn't find collider template " + t.Type.colliderType.ID + " with correct dir and rot for " + t.ID + "!"); }
				}
			}

			// check every's bloxel side with every bloxel
			// TODO: make the blocks only when needed - will be too much otherwise?
			// the following includes $MISSING!
			for (int i = 0, tc = templates.Count; i < tc; ++i) {
				templates[i].InitSidesDataClipped(tc);
				for (int j = 0; j < tc; ++j) {
					templates[i].CreateSideBySideData(templates[j]);
				}
			}

			// TODO reimplement
			// hidden bloxel mesh containers are created for colliding tests
			//		hiddenBloxelMeshContainers = new BloxelTemplateTransformDictionary(templateCount);
			//		foreach (var vt in templates) {
			//			if (vt.Mesh == null || vt.Mesh.vertexCount == 0) { continue; }
			//			var go = new GameObject("<Hidden Bloxel Mesh " + vt.ID + ">")
			//			{
			//				hideFlags = HideFlags.HideInHierarchy,
			//				layer = LayerMask.NameToLayer("Default") // TODO change
			//			};
			//			// TODO go.transform.SetParent(transform);
			//			go.transform.position = Vector3.down * 10000f + Vector3.right * hiddenBloxelMeshContainers.Count * 3f;
			//			go.AddComponent<MeshFilter>().sharedMesh = vt.Mesh;
			//			go.AddComponent<MeshCollider>();
			//			hiddenBloxelMeshContainers.Add(vt, go.transform);
			//		}

			SaveTemplates();

			float elapsed = (float)((System.DateTime.Now - dateTime).TotalSeconds);
			UnityEngine.Debug.Log("prepared blocks within " + elapsed.ToString("0.####") + " seconds for " + Types.Length + " types, resulting in " + templates.Count + " templates");
		}

		/// <summary>
		/// Prepare all the provided textures and pack them into a texture atlas
		/// </summary>
		/// <param name="reset"></param>
		public void PrepareTextures(bool reset = false) {
#if UNITY_EDITOR
			if (!AssetDatabase.IsValidFolder(ASSETS_PATH_RESOURCES + "/Generated/BloxelTextures")) {
				AssetDatabase.CreateFolder(ASSETS_PATH_RESOURCES + "/Generated", "BloxelTextures");
			}
			foreach (var res in Resources.LoadAll<BloxelTexture>("Generated/BloxelTextures")) {
				if (res.name == "$MISSING") { continue; }
				AssetDatabase.DeleteAsset(ASSETS_PATH_RESOURCES + "/Generated/BloxelTextures/" + res.name + ".asset");
			}

			if (!AssetDatabase.IsValidFolder(ASSETS_PATH_RESOURCES + "/Generated/TextureAtlases")) {
				AssetDatabase.CreateFolder(ASSETS_PATH_RESOURCES + "/Generated", "TextureAtlases");
			}
			foreach (var res in Resources.LoadAll<Texture>("Generated/TextureAtlases")) {
				AssetDatabase.DeleteAsset(ASSETS_PATH_RESOURCES + "/Generated/TextureAtlases/" + res.name + ".png");
			}
#endif

			var resources = Resources.LoadAll<BloxelTexture>("BloxelTextures");
			var texDefs = new List<BloxelTexture>(resources.Length);
			TextureShelves.Clear();
			foreach (var r in resources) {
				texDefs.Add(r);
				if (!TextureShelves.Contains(r.Shelf)) { TextureShelves.Add(r.Shelf); }
			}
			var countBTs = texDefs.Count;

			// the texture for when there is a texture missing ...
			if (missingTexture == null || missingTexture.texture == null) {
				// TODO should not happen here
				missingTexture = CreateInstance<BloxelTexture>();
				missingTexture.name = missingTexture.ID = "$MISSING";
				missingTexture.flags = missingTexture.neighbourFlags = 0;
				missingTexture.noiseStrength = 99999f;
				missingTexture.noiseScale = 99999f;
				// TODO should not happen here
				var nullSize = 15;
				missingTexture.maxPixelWidth = missingTexture.maxPixelHeight = 0;
				missingTexture.texture = new Texture2D(nullSize, nullSize) { name = "$MISSING Texture" };
				var nullColors = new Color32[nullSize * nullSize];
				for (int y = 0, i = 0; y < nullSize; ++y) {
					for (int x = 0; x < nullSize; ++x, ++i) {
						nullColors[i] = (x == y || x == nullSize - 1 - y || x == 0 || y == 0 || x == nullSize - 1 || y == nullSize - 1) ? new Color32(255, 255, 255, 255) : new Color32(255, 0, 255, 255);
					}
				}
				missingTexture.texture.SetPixels32(nullColors);
				missingTexture.texture.Apply();
				SaveTexture(missingTexture, "$MISSING");
			}

			missingTexture.tempIndexInAtlas = -1;
			countBTs += 1;

			this.textures.Clear();
			this.textures.Add(missingTexture);

			// clear textures, add "missing" texture
			for (int i = 0, missingAdded = 0; i < texAtlases.Length; ++i) {
				if (texAtlases[i] == null) { continue; }
				texAtlases[i].Textures.Clear();
				if (missingAdded == 0) { // add "missing" texture only to first atlas
					texAtlases[i].Textures.Add(missingTexture);
					missingTexture.texAtlasIdx = i;
					missingAdded = 1;
				}
			}

			// sort textures into their wanted atlases
			foreach (var td in texDefs) {
				if (td.texAtlasIdx >= 0 && td.texAtlasIdx < texAtlases.Length && texAtlases[td.texAtlasIdx] != null) {
					this.textures.Add(td);
					texAtlases[td.texAtlasIdx].Textures.Add(td);
				}
				if (reset) {
					td.tempIndexInAtlas = -1;
					td.tempProcessedTex = TextureWithDims.none;
				}
			}

			// all the textures in atlases!
			var tempAtlasInfos = new Dictionary<int, TemporaryAtlasInfo>();
			for (int i = 0; i < countBTs; ++i) {
				var bt = this.textures[i];
				var tas = texAtlases[bt.texAtlasIdx];
				if (!tempAtlasInfos.TryGetValue(bt.texAtlasIdx, out var tai)) {
					tai = tempAtlasInfos[bt.texAtlasIdx] = new TemporaryAtlasInfo() { idx = bt.texAtlasIdx };
					for (int p = 0; p < tas.Properties.Length; ++p) { tai.textures.Add(new List<Texture2D>()); }
				}

				int owidth = -1, oheight = -1;

				// iterate over all properties:
				for (int propIdx = 0; propIdx < tas.Properties.Length; ++propIdx) {
					var processTex = bt.texture;
					if (propIdx > 0) {
						if (bt.texturesSecondary.Length < propIdx || bt.texturesSecondary[propIdx - 1] == null) {
							var name = tas.Properties[propIdx].ToLower();
							processTex = Instantiate(name.Contains("bump") || name.Contains("normal") ? Texture2D.normalTexture : Texture2D.blackTexture);
						}
						else {
							processTex = bt.texturesSecondary[propIdx - 1];
						}
					}
					else if (processTex == null) {
						Debug.LogWarning(bt.ID + " has no main texture!", bt);
						continue;
					}

					int ow = processTex.width; // original dimensions
					int oh = processTex.height;

					var changeScale = false;
					if (tas.AsTexArray) {
						var tw = tas.TexArraySize.x * (bt.cutInTexArray ? bt.countX : 1); 
						var th = tas.TexArraySize.y * (bt.cutInTexArray ? bt.countY : 1);
						if (tw != ow || th != oh) {
							ow = tw;
							oh = th;
							changeScale = true;
						}
					}
					else {
						if (propIdx == 0) {
							var hmpw = bt.maxPixelWidth > 0 && bt.maxPixelWidth != processTex.width;
							var hmph = bt.maxPixelHeight > 0 && bt.maxPixelHeight != processTex.height;
							if (hmpw) { ow = bt.maxPixelWidth; changeScale = true; }
							if (hmph) { oh = bt.maxPixelHeight; changeScale = true; }
							owidth = ow;
							oheight = oh;
						}
						else if (ow != owidth || oh != oheight) {
							ow = owidth;
							oh = oheight;
							changeScale = true;
						}
					}

					var btf = this.textures.Find(t => t != bt && t.texAtlasIdx == bt.texAtlasIdx && t.tempProcessedTex.Is(ow, oh, bt.texture));
					if (btf != null) {
						// TODO this might cause problems with textures on different atlasses
						//Debug.Log(bt.name + " changed temp! " + bt.tempIndexInAtlas + " -> " + btf.tempIndexInAtlas + " ... " + bt.tempProcessedTex.tex + " -> " + btf.tempProcessedTex.tex);
						bt.tempIndexInAtlas = btf.tempIndexInAtlas;
						bt.tempProcessedTex = btf.tempProcessedTex;
						continue;
					}

					if (changeScale) {
						processTex = Texture2D.Instantiate(processTex);
						if (tas.AtlasFilterMode == FilterMode.Point && (ow >= owidth || oh >= oheight)) {
							TextureScale.Point(processTex, ow, oh);
						}
						else {
							TextureScale.Bilinear(processTex, ow, oh); // TODO?
						}
						processTex.Apply();
					}
					var oPixels = processTex.GetPixels32();

					var hasTransparency = false;
					if (propIdx == 0) {
						var alpha = 1f - bt.transparency;
						for (int k = 0; k < oPixels.Length; ++k) {
							var a = alpha * (oPixels[k].a / 255f);
							if (a < 1f) {
								hasTransparency = tai.mainHasTransparency = true;
								oPixels[k].a = (byte)(255 * a);
							}
						}
					}

					if (tas.AsTexArray) {
						var aTex = new Texture2D(ow, oh, hasTransparency ? TextureFormat.ARGB32 : TextureFormat.RGB24, true);
						aTex.SetPixels32(oPixels);
						aTex.Apply();
						tai.textures[propIdx].Add(aTex);
					}
					else {
						int nw = TEXTURE_PADDING * 2 + ow, nh = TEXTURE_PADDING * 2 + oh; // new width and height
						var padTex = new Texture2D(nw, nh, hasTransparency ? TextureFormat.ARGB32 : TextureFormat.RGB24, true);
						padTex.SetPixels32(TEXTURE_PADDING, TEXTURE_PADDING, ow, oh, oPixels);

						var tempPixels = new Color32[Mathf.Max(TEXTURE_PADDING * TEXTURE_PADDING, ow, oh)];

						var tp = new List<Color32>(); // left side
						for (int y = 0; y < oh; ++y) { for (int x = -TEXTURE_PADDING; x < 0; ++x) { tp.Add(oPixels[y * ow]); } }
						padTex.SetPixels32(0, TEXTURE_PADDING, TEXTURE_PADDING, oh, tp.ToArray());
						tp.Clear(); // right side
						for (int y = 0; y < oh; ++y) { for (int x = 0; x < TEXTURE_PADDING; ++x) { tp.Add(oPixels[(ow - 1) + y * ow]); } }
						padTex.SetPixels32(ow + TEXTURE_PADDING, TEXTURE_PADDING, TEXTURE_PADDING, oh, tp.ToArray());
						tp.Clear(); // bottom side
						for (int y = -TEXTURE_PADDING; y < 0; ++y) { for (int x = -TEXTURE_PADDING; x < ow + TEXTURE_PADDING; ++x) { tp.Add(oPixels[Mathf.Clamp(x, 0, ow - 1)]); } }
						padTex.SetPixels32(0, 0, ow + TEXTURE_PADDING * 2, TEXTURE_PADDING, tp.ToArray());
						tp.Clear(); // top side
						for (int y = 0; y < TEXTURE_PADDING; ++y) { for (int x = -TEXTURE_PADDING; x < ow + TEXTURE_PADDING; ++x) { tp.Add(oPixels[Mathf.Clamp(x, 0, ow - 1) + (oh - 1) * ow]); } }
						padTex.SetPixels32(0, oh + TEXTURE_PADDING, ow + TEXTURE_PADDING * 2, TEXTURE_PADDING, tp.ToArray());

						padTex.Apply();
						tai.textures[propIdx].Add(padTex);
					}
					bt.tempProcessedTex = new TextureWithDims(bt.texture, ow, oh);
				}
				if (bt.tempIndexInAtlas == -1) {
					bt.tempIndexInAtlas = tai.indices;
					tai.indices += 1;
				}
			}

			var changedMaterials = new HashSet<Material>();
			foreach (var tai in tempAtlasInfos) {
				var tas = texAtlases[tai.Key];
				for (int propIdx = 0; propIdx < tas.Properties.Length; ++propIdx) {
					if (tas.AsTexArray) {
						var tCount = 0;
						for (int i = 0; i < tai.Value.textures[propIdx].Count; ++i) {
							var bt = this.textures.Find(t => t.texAtlasIdx == tai.Key && t.tempIndexInAtlas == i);
							tCount += bt.cutInTexArray ? (bt.countX * bt.countY) : 1;
						}
						var array = new Texture2DArray(tas.TexArraySize.x, tas.TexArraySize.y, tCount, tai.Value.mainHasTransparency ? TextureFormat.ARGB32 : TextureFormat.RGB24, true) {
							name = "<ARRAY " + tai.Key + " " + tas.ID + (propIdx > 0 ? " " + tas.Properties[propIdx] : "") + ">",
							filterMode = tas.AtlasFilterMode,
							wrapMode = TextureWrapMode.Repeat,
							anisoLevel = 16
						};
						// create the array
						if (propIdx == 0) {
							int arrayIdx = 0;
							int i = 0;
							foreach (var tex in tai.Value.textures[propIdx]) {
								var bt = this.textures.Find(t => t.texAtlasIdx == tai.Key && t.tempIndexInAtlas == i);
								++i;
								if (bt == null) { Debug.LogError("Something went wrong creating an tex array"); return; }
								
								if (bt.cutInTexArray) {
									bt.rect = new Rect(0f, 0f, 1f, 1f);
									bt.pos = new Vector2[bt.countX * bt.countY];
									bt.size = new Vector2(1f, 1f);
									bt.arrayIdx = new int[bt.countX * bt.countY];
									for (int y = 0, si = 0; y < bt.countY; ++y) {
										for (int x = 0; x < bt.countX; ++x, ++si) {
											bt.pos[si] = new Vector2(0, 0);
											bt.arrayIdx[si] = arrayIdx;
											for (int m = 0, w = array.width, h = array.height; m < array.mipmapCount; ++m, w /= 2, h /= 2) {
												Graphics.CopyTexture(
													tex, 0, m, x * w, (bt.countY - 1 - y) * h, w, h,
													array, arrayIdx, m, 0, 0);
											}
											++arrayIdx;
										}
									}
								}
								else {
									float wc = 1f / bt.countX, hc = 1f / bt.countY;
									Graphics.CopyTexture(tex, 0, array, arrayIdx);
									bt.rect = new Rect(0f, 0f, 1f, 1f);
									bt.pos = new Vector2[bt.countX * bt.countY];
									bt.size = new Vector2(wc, hc);
									bt.arrayIdx = new int[bt.countX * bt.countY];
									for (int y = 0, si = 0; y < bt.countY; ++y) {
										for (int x = 0; x < bt.countX; ++x, ++si) {
											bt.pos[si] = new Vector2(wc * x, hc * (bt.countY - 1 - y));
											bt.arrayIdx[si] = arrayIdx;
										}
									}
									++arrayIdx;
								}
							}
						}
#if UNITY_EDITOR
						var path = "/Generated/TextureAtlases/TexAtlas " + tas.ID + (propIdx > 0 ? " " + tas.Properties[propIdx] : "") + ".asset";
						if (!AssetDatabase.IsValidFolder(ASSETS_PATH_RESOURCES + System.IO.Path.GetDirectoryName(path))) {
							AssetDatabase.CreateFolder(ASSETS_PATH_RESOURCES + "/Generated", "TextureAtlases");
						}
						AssetDatabase.CreateAsset(array, ASSETS_PATH_RESOURCES + path);
#endif

						tas.Material.SetTexture(tas.Properties[propIdx], array);
						if (propIdx == 0 && changedMaterials.Contains(tas.Material)) {
							Debug.LogWarning("Assigned texture array to material a second time!!! Give each TextureAtlas in the ProjectSettings a different material!");
						}
						changedMaterials.Add(tas.Material);
#if UNITY_EDITOR
						EditorUtility.SetDirty(tas);
#endif
					}
					else {
						var atlas = new Texture2D(1024, 1024) {
							name = "<ATLAS " + tai.Key + " " + tas.ID + (propIdx > 0 ? " " + tas.Properties[propIdx] : "") + ">",
							filterMode = tas.AtlasFilterMode,
							wrapMode = TextureWrapMode.Clamp,
#if UNITY_EDITOR
							alphaIsTransparency = (propIdx == 0 && tai.Value.mainHasTransparency),
#endif
							anisoLevel = 16
						};
						// create the atlas
#if UNITY_EDITOR
						var rects = atlas.PackTextures(tai.Value.textures[propIdx].ToArray(), 0, 8192, false);
#else
						// TODO does this work reliably?
						var rects = atlas.PackTextures(tai.Value.textures[propIdx].ToArray(), 0, 8192, true);
#endif
						var savedAtlas = SaveTextureAsPNG(tas, atlas, "Generated/TextureAtlases/TexAtlas " + tas.ID + (propIdx > 0 ? " " + tas.Properties[propIdx] : ""));
						var padX = TEXTURE_PADDING / (float)atlas.width;
						var padY = TEXTURE_PADDING / (float)atlas.height;
						//for (var iter_tex = cur.Value.GetEnumerator(); iter_tex.MoveNext(); ) {
						if (propIdx == 0) {
							for (int i = 0; i < countBTs; ++i) {
								var bt = this.textures[i];
								if (bt.texAtlasIdx != tai.Key) { continue; }
								// assign the rects
								//Debug.Log(bt.ID + " - " + rects.Length + " " + bt.tempIndexInAtlas);
								var rect = rects[bt.tempIndexInAtlas]; // rects[indices[vt.meshIdx][i]];
								rect.x += padX; rect.width -= padX * 2f; // removes the padding
								rect.y += padY; rect.height -= padY * 2f;
								//Debug.Log(vt.ID + ":" + vt.texture.name + " -> " + vt.rect + " " + vt.countX + "/" + vt.countY);
								float w = rect.width / bt.countX;
								float h = rect.height / bt.countY;
								bt.pos = new Vector2[bt.countX * bt.countY];
								bt.size = new Vector2(w, h); // can be reduced to one?
								bt.arrayIdx = new int[bt.countX * bt.countY];
								for (int y = 0, si = 0; y < bt.countY; ++y) {
									//vt.pos[y] = new Vector2[vt.countX];
									//vt.size[y] = new Vector2[vt.countX];
									for (int x = 0; x < bt.countX; ++x, ++si) {
										bt.pos[si] = new Vector2(rect.x + w * x, rect.y + h * (bt.countY - 1 - y));
										bt.arrayIdx[si] = 0;
									}
								}
								bt.rect = rect;
								//Debug.Log("   px: " + vt.rect.x * atlas.width + "/" + vt.rect.y * atlas.height + " - " + vt.rect.width * atlas.width + "/" + vt.rect.height * atlas.height);
								//vt.atlas = savedAtlas;
							}
						}

						//tas.Atlases.Add(savedAtlas);
						//tas.Material.mainTexture = savedAtlas; // TODO
						tas.Material.SetTexture(tas.Properties[propIdx], savedAtlas);
						if (propIdx == 0 && changedMaterials.Contains(tas.Material)) {
							Debug.LogWarning("Assigned texture atlas to material a second time!!! Give each TextureAtlas in the ProjectSettings a different material!");
						}
						changedMaterials.Add(tas.Material);
#if UNITY_EDITOR
						EditorUtility.SetDirty(tas);
#endif
					}
				}
			}
#if UNITY_EDITOR
			for (int i = 0; i < countBTs; ++i) {
				EditorUtility.SetDirty(this.textures[i]);
			}

			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
#endif

			// TODO: free not used textures in ram
			// TODO: create my own mipmaps

			// test:
			/* var testMat = new Material(material);
			testMat.mainTexture = atlas;
			var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
			go.GetComponent<Renderer>().material = testMat;
			go.transform.position = Vector3.up * 6f; */

		}

		// https://answers.unity.com/questions/1331297/how-to-save-a-texture2d-into-a-png.html
		public static Texture SaveTextureAsPNG(TextureAtlasSettings settings, Texture2D texture, string resourcePath) {
			var pngPath = ASSETS_PATH_RESOURCES + "/" + resourcePath + ".png";
			var fullPath = Application.dataPath + "/../" + pngPath;
			byte[] _bytes = texture.EncodeToPNG();
			if (System.IO.File.Exists(fullPath)) { System.IO.File.Delete(fullPath); }
			System.IO.File.WriteAllBytes(fullPath, _bytes);
#if UNITY_EDITOR
			UnityEditor.AssetDatabase.ImportAsset(pngPath, ImportAssetOptions.ForceUpdate);
			var importer = AssetImporter.GetAtPath(pngPath) as TextureImporter;
			importer.sRGBTexture = true;
			importer.textureCompression = settings.AtlasCompression;
			importer.filterMode = settings.AtlasFilterMode;
			importer.wrapMode = TextureWrapMode.Clamp;
			importer.maxTextureSize = 16384;
#endif
			//Debug.Log(_bytes.Length/1024  + "Kb was saved as: " + resourcePath);
			return Resources.Load<Texture>(resourcePath);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// COLLISION TEST
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//public bool GetRandomPositionInside(BloxelTemplate vt, out Vector3 pos) {
		//	var layerMask = LayerMask.GetMask("Transformators");
		//	Transform trans;
		//	if (hiddenBloxelMeshContainers.TryGetValue(vt, out trans)) {
		//		var r = new Vector3(Random.Range(-0.49f, 0.49f), Random.Range(-0.49f, 0.49f), Random.Range(-0.49f, 0.49f)) + trans.position;
		//		var count1 = Physics.RaycastNonAlloc(new Ray(r, Vector3.forward), raycastHits, 2f, layerMask);
		//		var count2 = Physics.RaycastNonAlloc(new Ray(r + Vector3.forward * 2f, Vector3.back), raycastHits, 2f, layerMask);
		//		if ((count1 + count2) % 2 == 1) {
		//			pos = r - trans.position;
		//			return true;
		//		}
		//	}
		//	pos = Vector3.zero;
		//	return false;
		//}

	}

}