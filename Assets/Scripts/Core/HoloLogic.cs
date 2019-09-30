using Assets.Scripts;
using Assets.Scripts.Builder;
using Assets.Scripts.Model;
using Assets.Scripts.UI;
using HelloHolo;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.InputModule.Utilities.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace Core
{
    public enum ShotMode
    {
        Ceiling,
        Floor,
        Wall,
        Window,
        Door
    }
    class HoloLogic : MonoBehaviour
    {
        private IHoloRepository _repo;
        private IStructureBuilder _structureBuilder;
        private IOpeningBuilder _openingBuilder;
        private IShootHolo _shooter;
        private ShotMode mode;

        private KeywordRecognizer recog;

        private void Awake()
        {
            _repo = new HoloRepository();
            BaseModel._repo = _repo;
            _openingBuilder = new OpeningBuilder(_repo);
            _structureBuilder = new StructureBuilder(_repo);
            _structureBuilder.Init();
            Workspace.Instance.SetDependencies(_structureBuilder);
            _shooter = gameObject.AddComponent<ShootHolo>();
            _shooter.HitAction += HitReceived;
            SwitchMode(ShotMode.Floor);
        }

        private void Start()
        {
            var asd = Enum.GetNames(typeof(ShotMode)).Union(new string[] { "object" }).ToArray();
            recog = new KeywordRecognizer(asd);
            recog.OnPhraseRecognized += OnSpeechKeywordRecognized;
            recog.Start();
        }


        private void HoloLogic_TwoHandedHappened()
        {
            ShowCreatorMenu();
        }

        private void ShowCreatorMenu()
        {

        }

        private void HitReceived(RaycastHit hit)
        {
            if (!isPlaceMode)
            {
                switch (mode)
                {
                    case ShotMode.Ceiling:
                        _structureBuilder.CreateCeiling(hit.point);
                        SwitchMode(ShotMode.Wall);
                        GUIRoot.Instance.AnimateDialog("Ceiling shot was successful");
                        break;
                    case ShotMode.Floor:
                        _structureBuilder.CreateFloor(hit.point);
                        SwitchMode(ShotMode.Ceiling);
                        GUIRoot.Instance.AnimateDialog("Floor shot was successful");
                        break;
                    case ShotMode.Wall:
                        _structureBuilder.CreateVertex(hit.point);
                        break;
                    case ShotMode.Window:
                        _openingBuilder.ShotEdge(hit.point, OpeningType.Window,
                            _repo.GetModel<Wall>(hit.transform.gameObject.GetComponentInParent<Assets.Scripts.View.Wall>().name));
                        break;
                    case ShotMode.Door:
                        _openingBuilder.ShotEdge(hit.point, OpeningType.Door,
                            _repo.GetModel<Wall>(hit.transform.gameObject.GetComponentInParent<Assets.Scripts.View.Wall>().name));
                        break;
                }
                GUIRoot.Instance.ShotHappened();
                GetComponent<AudioSource>().Play();
            }
            else
            {
                PlaceObject(hit);
            }
        }

        private void PlaceObject(RaycastHit hit)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>(placeObjectPath),  hit.point, Quaternion.identity);
            go.transform.position = hit.point;
            SwitchToPlaceMode(false);
            SwitchMode("wall");
        }

        public void SwitchMode(int mode)
        {
            SwitchMode((ShotMode)mode);
        }

        public void SwitchMode(ShotMode newMode)
        {
            if (mode == newMode) return;

            mode = newMode;
            LayerMask mask;
            if (newMode == ShotMode.Door || newMode == ShotMode.Window)
            {
                mask = 1 << (int)LayerEnums.Wall;
            }
            else
            {
                mask = 1 << (int)LayerEnums.SpatialUnderstanding;
            }
            GUIRoot.Instance.ChangeSelectedMode(mode);
            _shooter.ChangeLayer(mask.value);
            GetComponent<TextToSpeech>().StartSpeaking(mode.ToString() + " shot activated");
        }

        public void SwitchMode(string txt)
        {
            if (mode.ToString().ToLower() == txt || (isPlaceMode && txt == "object"))
                return;

            GUIRoot.Instance.EnableWindowshoot(false);
            GUIRoot.Instance.EnableDoorshoot(false);

            if ("object" == txt)
            {
                SwitchToPlaceMode(true);
                SelectObject("chair");
                GetComponent<TextToSpeech>().StartSpeaking("Object placing activated");
            }
            else
            {
                SwitchToPlaceMode(false);

                switch (txt)
                {
                    case "door":
                        SwitchMode(ShotMode.Door);
                        GUIRoot.Instance.EnableDoorshoot(true);
                        break;
                    case "window":
                        SwitchMode(ShotMode.Window);
                        GUIRoot.Instance.EnableWindowshoot(true);
                        break;
                    case "wall":
                        SwitchMode(ShotMode.Wall);
                        break;
                    case "floor":
                        SwitchMode(ShotMode.Floor);
                        break;
                    case "ceiling":
                        SwitchMode(ShotMode.Ceiling);
                        break;
                }
            }

            
        }

        private void SwitchToPlaceMode(bool isPlaceMode)
        {
            //GUIRoot.Instance.EnableObjectLibrary(isPlaceMode);
            this.isPlaceMode = isPlaceMode;
        }

        public void OnSpeechKeywordRecognized(PhraseRecognizedEventArgs args)
        {            
            string txt = args.text.ToLower();
            SwitchMode(txt);
        }

        public void SelectObject(string path)
        {
            path = "Models/Prefabs/" + path + "/prefab";
            this.placeObjectPath = path;
            GUIRoot.Instance.EnableObjectLibrary(false);
        }
        private string placeObjectPath;
        private bool isPlaceMode;

        #region mesh save
#if UNITY_EDITOR

        private int incr = 0;

        void SaveAsset(GameObject parent)
        {
            foreach (Transform tr in parent.GetComponentsInChildren<Transform>())
            {
                var mf = tr.GetComponent<MeshFilter>();
                if (mf)
                {
                    tr.GetComponent<MeshCollider>().sharedMesh = mf.sharedMesh;
                    var savePath = "Assets/savedMesh/" + parent.name + "/" + incr++ + ".asset";
                    Debug.Log("Saved Mesh to:" + savePath);
                    AssetDatabase.CreateAsset(mf.mesh, savePath);
                }
            }
            var prefab = PrefabUtility.CreateEmptyPrefab("Assets/savedMesh/" + parent.name + "/" + parent.name + "asdasd" + ".prefab");
            PrefabUtility.ReplacePrefab(parent, prefab);
            AssetDatabase.Refresh();
        }
#endif
        #endregion
    }
}
