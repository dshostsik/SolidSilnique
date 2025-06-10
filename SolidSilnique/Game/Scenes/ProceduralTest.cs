using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SolidSilnique.Core;
using SolidSilnique.Core.ArtificialIntelligence;
using SolidSilnique.Core.Components;
using SolidSilnique.ProcderuralFoliage;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using GUIRESOURCES;
using Microsoft.Xna.Framework.Input;
using SolidSilnique.Core.Diagnostics;
using SolidSilnique.Core.Animation;
using static System.Formats.Asn1.AsnWriter;


namespace SolidSilnique.GameContent;

class ProceduralTest : Scene
{
    //PROCEDURAL HELPERS
    List<Model> models = new List<Model>();
    List<Texture2D> textures = new List<Texture2D>();
    List<Model> treeModels = new List<Model>();
    List<Texture2D> treeTextures = new List<Texture2D>();


    public EnvironmentObject enviro = new EnvironmentObject();
    KeyboardState kState = new KeyboardState();
    private BossRhythymUI bossRhythym = new BossRhythymUI();
    SpriteBatch spriteBatch = new SpriteBatch(EngineManager.graphics);
    public GUI rhythymGui;

    ContentManager content;

    // let's assume it as a player object so far
    private GameObject gab;
    private GameObject enemy;
    private int _enemyHP = 100;
    private bool turnedOn = false;

    public ProceduralTest()
    {
    }

    public override void LoadContent(ContentManager Content)
    {
        //loadedModels.Add("drzewo", Content.Load<Model>("drzewo2"));
        loadedModels.Add("deimos", Content.Load<Model>("deimos"));
        loadedModels.Add("plane", Content.Load<Model>("plane"));
        loadedModels.Add("cube", Content.Load<Model>("cube"));
        loadedModels.Add("cone", Content.Load<Model>("cone"));
        loadedModels.Add("sphere", Content.Load<Model>("sphere"));
        loadedModels.Add("levelTest", Content.Load<Model>("level_ground"));
        loadedModels.Add("brzewno1/brzewno", Content.Load<Model>("brzewno1/brzewno"));
        loadedModels.Add("brzewno3/brzewno", Content.Load<Model>("brzewno3/brzewno3"));
        //loadedModels.Add("drzewo/drzewo", Content.Load<Model>("drzewo/drzewo"));
        loadedModels.Add("trent", Content.Load<Model>("trent"));
        loadedModels.Add("dodik", Content.Load<Model>("dodik"));
        
        loadedTextures.Add("deimos", Content.Load<Texture2D>("deimos_texture"));
        loadedTextures.Add("testTex", Content.Load<Texture2D>("testTex"));
        loadedTextures.Add("simpleGreen", Content.Load<Texture2D>("simpleGreen"));
        loadedTextures.Add("simpleBlack", Content.Load<Texture2D>("simpleBlack"));
        loadedTextures.Add("gabTex", Content.Load<Texture2D>("Textures/gab_tex"));
        loadedTextures.Add("gabNo", Content.Load<Texture2D>("Textures/gab_no"));
        loadedTextures.Add("gabRo", Content.Load<Texture2D>("Textures/gab_ro"));
        loadedTextures.Add("gabAo", Content.Load<Texture2D>("Textures/gab_ao"));

        loadedTextures.Add("eye", Content.Load<Texture2D>("Textures/eye_tex"));

        loadedTextures.Add("leafTex", Content.Load<Texture2D>("Textures/leaf_diffuse"));

        loadedTextures.Add("brzewno1/diffuse", Content.Load<Texture2D>("brzewno1/diffuse"));
        loadedTextures.Add("brzewno1/normal", Content.Load<Texture2D>("brzewno1/normal"));
        loadedTextures.Add("brzewno1/ao", Content.Load<Texture2D>("brzewno1/ao"));
        loadedTextures.Add("brzewno1/glossy", Content.Load<Texture2D>("brzewno1/glossy"));

        loadedTextures.Add("brzewno3/diffuse", Content.Load<Texture2D>("brzewno3/diffuse"));
        loadedTextures.Add("brzewno3/normal", Content.Load<Texture2D>("brzewno3/normal"));
        loadedTextures.Add("brzewno3/ao", Content.Load<Texture2D>("brzewno3/ao"));
        loadedTextures.Add("brzewno3/glossy", Content.Load<Texture2D>("brzewno3/glossy"));

        loadedTextures.Add("drzewo/diffuse", Content.Load<Texture2D>("drzewo/diffuse"));
        loadedTextures.Add("drzewo/normal", Content.Load<Texture2D>("drzewo/normal"));
        loadedTextures.Add("drzewo/ao", Content.Load<Texture2D>("drzewo/ao"));
        loadedTextures.Add("drzewo/glossy", Content.Load<Texture2D>("drzewo/glossy"));

        loadedTextures.Add("trent/diffuse", Content.Load<Texture2D>("trent_fire/low_material_Base_color"));
        loadedTextures.Add("trent/normal", Content.Load<Texture2D>("trent_fire/low_material_Normal_DirectX"));
        
        loadedTextures.Add("dodik_texture", Content.Load<Texture2D>("dodik_texture"));
        //loadedTextures.Add("trent/ao", Content.Load<Texture2D>("trent_fire/PM3D_Cylinder3D_10_Mixed_AO"));
        //loadedTextures.Add("trent/roughness", Content.Load<Texture2D>("trent_fire/PM3D_Cylinder3D_10_Coat_roughness"));

        models.Add(Content.Load<Model>("pModels/Rock1"));
        models.Add(Content.Load<Model>("pModels/Branch"));
        models.Add(Content.Load<Model>("pModels/BushBig"));
        models.Add(Content.Load<Model>("pModels/BushBig"));
        models.Add(Content.Load<Model>("pModels/BushBig"));
        models.Add(Content.Load<Model>("pModels/BushBig"));
        models.Add(Content.Load<Model>("pModels/BushSmall"));
        models.Add(Content.Load<Model>("pModels/BushSmall"));
        models.Add(Content.Load<Model>("pModels/BushSmall"));
        //models.Add(Content.Load<Model>("pModels/Log"));
        models.Add(Content.Load<Model>("pModels/Stump"));

        models.Add(Content.Load<Model>("brzewno1/brzewno"));
        models.Add(Content.Load<Model>("brzewno3/brzewno3"));
        textures.Add(loadedTextures["leafTex"]);
        textures.Add(loadedTextures["deimos"]);

        treeModels.Add(Content.Load<Model>("pModels/tree1"));
        treeModels.Add(Content.Load<Model>("pModels/Tree2"));
        treeTextures.Add(Content.Load<Texture2D>("Textures/tree1_diffuse"));
        treeTextures.Add(Content.Load<Texture2D>("Textures/tree2_diffuse"));
        //treeTextures.Add(Content.Load<Texture2D>("Textures/gab_tex"));
        loadedModels.Add("tower", Content.Load<Model>("Models/tower"));
        loadedModels.Add("eMonster1", Content.Load<Model>("Models/monstr"));
        content = Content;
    }

    public override void Setup()
    {
        environmentObject = new EnvironmentObject();
        environmentObject.Generate("Map1", content, 3, 60, 3);

        ProceduralGrass newProc =
            new ProceduralGrass(models, textures, treeModels, treeTextures, content, environmentObject);
        Task task1 = Task.Run(() => newProc.precomputeNoise());

        GameObject go = new GameObject("Camera");
        go.transform.position = new Vector3(250, 3f, 250);
        CameraComponent cam = new CameraComponent();
        cam.SetMain();
        go.AddComponent(cam);
        //go.AddComponent(new SphereColliderComponent(3f));
        this.AddChild(go);

        go = new GameObject("ground");
        go.transform.position = new Vector3(250, 0, 250);
        go.transform.scale = new Vector3(500, 1, 500);
        //go.model = loadedModels["plane"];
        go.texture = loadedTextures["simpleGreen"];
        go.AddComponent(new PlaneColliderComponent(new Vector3(0, 1, 0), true));
        this.AddChild(go);
        Task.WhenAll(task1).Wait(); //:O

        newProc.GenerateObjects();
        List<GameObject> goList = newProc.createdObjects;

        for (int a = 0; a < goList.Count; a++)
        {
            this.AddChild(goList[a]);
        }


        GameObject goTest = new GameObject("Deimos");


        goTest.transform.position = new Vector3(150, 2.5f, 150);
        goTest.model = loadedModels["levelTest"];
        goTest.texture = loadedTextures["deimos"];
        goTest.AddComponent(new SphereColliderComponent(3.5f, true));


        this.AddChild(goTest);

        go = new GameObject("Testak");
        go.transform.position = new Vector3(5, 2.5f, -5);
        go.model = loadedModels["deimos"];
        go.texture = loadedTextures["deimos"];


        //go.AddComponent(new DebugMoveComponent()); //<-- Dodawanie componentów
        go.AddComponent(new SphereColliderComponent(3.5f));

        this.AddChild(go);


        GameObject go2 = new GameObject("Square4");
        go2.transform.position = new Vector3(0, 5, 0);
        go2.transform.scale = new Vector3(0.75f);
        go2.model = loadedModels["deimos"];
        go2.texture = loadedTextures["deimos"];
        go.AddChild(go2);

        gab = new GameObject("gab");
        gab.transform.position = new Vector3(250, 15, 220);

        gab.transform.scale = new Vector3(1f);
        gab.model = loadedModels["dodik"];
        //gab.texture = loadedTextures["dodik_texture"];
        //gab.normalMap = loadedTextures["gabNo"];
        //gab.roughnessMap = loadedTextures["gabRo"];
        //gab.aoMap = loadedTextures["gabAo"];
        gab.AddComponent(new DebugMoveComponent());
        gab.AddComponent(new SphereColliderComponent(1));


        this.AddChild(gab);
        GameObject TPcam = new GameObject("cam");
        var tpcCamComp = new CameraComponent();
        TPcam.AddComponent(tpcCamComp);
        TPcam.transform.position = new Vector3(0, 5, -10);
        this.TPCamera = new Camera(tpcCamComp);

        gab.AddChild(TPcam);

        GameObject eye1 = new GameObject("eye1");
        eye1.transform.position = new Vector3(-0.25f * 2, 0.209f, 0.427f * 2);
        eye1.transform.scale = new Vector3(0.4f);
        eye1.model = loadedModels["sphere"];
        eye1.texture = loadedTextures["eye"];
        gab.AddChild(eye1);

        GameObject pupil1 = new GameObject("pupil1");
        pupil1.transform.position = new Vector3(0, 0, 0.427f * 2);
        pupil1.transform.scale = new Vector3(0.4f, 0.4f, 0.2f);
        pupil1.model = loadedModels["sphere"];
        pupil1.texture = loadedTextures["simpleBlack"];
        eye1.AddChild(pupil1);

        GameObject brow1 = new GameObject("brow1");
        brow1.transform.position = new Vector3(-0.25f * 2, 0.5f, 0.427f * 2);
        brow1.transform.scale = new Vector3(0.45f, 0.2f, 0.4f);
        brow1.transform.rotation = new Vector3(0f, 0, -20f);
        brow1.model = loadedModels["cube"];
        brow1.texture = loadedTextures["simpleBlack"];
        gab.AddChild(brow1);

        GameObject eye2 = new GameObject("eye2");
        eye2.transform.position = new Vector3(0.25f * 2, 0.209f, 0.427f * 2);
        eye2.transform.scale = new Vector3(0.4f);
        eye2.model = loadedModels["sphere"];
        eye2.texture = loadedTextures["eye"];
        gab.AddChild(eye2);

        GameObject pupil2 = new GameObject("pupil1");
        pupil2.transform.position = new Vector3(0, 0, 0.427f * 2);
        pupil2.transform.scale = new Vector3(0.4f, 0.4f, 0.2f);
        pupil2.model = loadedModels["sphere"];
        pupil2.texture = loadedTextures["simpleBlack"];
        eye2.AddChild(pupil2);

        GameObject brow2 = new GameObject("brow1");
        brow2.transform.position = new Vector3(0.25f * 2, 0.5f, 0.427f * 2);
        brow2.transform.scale = new Vector3(0.45f, 0.2f, 0.4f);
        brow2.transform.rotation = new Vector3(0f, 0, 20f);
        brow2.model = loadedModels["cube"];
        brow2.texture = loadedTextures["simpleBlack"];
        gab.AddChild(brow2);




        

        GameObject Tower = new GameObject("tower");
        Tower.transform.position = new Vector3(512, 80, 0);
        Tower.transform.scale = new Vector3(20, 80, 20);
        Tower.transform.rotation = new Vector3(0f, 0, 0f);
        Tower.model = loadedModels["tower"];
        Tower.texture = loadedTextures["eye"];
        this.AddChild(Tower);


        var cube = new GameObject("AnimatedCube");
        cube.model = loadedModels["deimos"];
        cube.texture = loadedTextures["deimos"];
        cube.transform.scale = new Vector3(1, 1, 1);

        var clip = new AnimationClip();
        // position: from (0,0,0) to (0,5,0) over 2s
        clip.PositionCurve.AddKey(new Keyframe<Vector3>(0f, new Vector3(0, 0, 0)));
        clip.PositionCurve.AddKey(new Keyframe<Vector3>(2f, new Vector3(0, 5, 0)));
        // rotation: yaw 0→360° over 2s
        clip.RotationCurve.AddKey(new Keyframe<Vector3>(0f, Vector3.Zero));
        clip.RotationCurve.AddKey(new Keyframe<Vector3>(2f, new Vector3(0, 360, 0)));
        // scale: pulse 1→2→1
        clip.ScaleCurve.AddKey(new Keyframe<Vector3>(0f, Vector3.One));
        clip.ScaleCurve.AddKey(new Keyframe<Vector3>(1f, Vector3.One * 2f));
        clip.ScaleCurve.AddKey(new Keyframe<Vector3>(2f, Vector3.One));

        var animator = new AnimatorComponent(clip, loop: true);
        cube.AddComponent(animator);
        animator.Play();

        // add to scene
        this.AddChild(cube);


        GameObject prevGeb = gab;
			for (int i = 0; i < 10; i++)
			{
				GameObject gogus = CreateGebus(new Vector3(150 + i*2, 2, 150 + i*2));
				gogus.GetComponent<Follower>().Target = prevGeb;
				if (i == 0) gogus.GetComponent<Follower>().SocialDistanceMultiplier = 4.0f;
				this.AddChild(gogus);
				prevGeb = gogus;
			}

        GameObject ziutek = CreateMovableObject("ziutek", 200,  200);
        ziutek.GetComponent<SphereColliderComponent>().boundingSphere.Radius = 2.0f;
        //ziutek.GetComponent<Follower>().Target = gab;
        this.AddChild(ziutek);
        
        rhythymGui = new GUI("Content/RhythymGui.xml", content);
        EngineManager.currentGui = rhythymGui;

        enemy = new GameObject("euzebiusz wiercibok");
        enemy.AddComponent(new SphereColliderComponent(3.5f, false));
        enemy.AddComponent(new Follower(enemy, 20f));
        enemy.AddComponent(new DebugMoveComponent());
        enemy.GetComponent<DebugMoveComponent>().move = false;
        enemy.model = loadedModels["trent"];
        enemy.texture = loadedTextures["trent/diffuse"];
        enemy.normalMap = loadedTextures["trent/normal"];
        enemy.transform.position = new Vector3(100, 0, 100);
        this.AddChild(enemy);

        this.AddChild(go);
    
    }

    public override void Update()
    {
        kState = Keyboard.GetState();

        if (SquaredDistanceBetweenEnemyAndPlayer() <=
            enemy.GetComponent<Follower>().SocialDistance * enemy.GetComponent<Follower>().SocialDistance * 3.0f)
        {
            enemy.GetComponent<Follower>().Target = gab;
        }

        if ((EnemyReachedPlayer() || kState.IsKeyDown(Keys.M)) && !turnedOn)
        {
            bossRhythym.Start(content, spriteBatch);
            turnedOn = true;
        }

        if (turnedOn)
            bossRhythym.Update();
        rhythymGui.progressBars[0].progress = bossRhythym.health;
        rhythymGui.texts[0].text = bossRhythym.ReturnScoresAndAccuracy().ToString();
        rhythymGui.texts[1].text = bossRhythym.combo.ToString();


        base.Update();
    }

    GameObject CreateGebus(Vector3 pos)
    {
        GameObject go = new GameObject("Gebus");

        go.transform.position = pos;
        go.transform.scale = new Vector3(0.75f);
        go.model = loadedModels["sphere"];
        go.texture = loadedTextures["gabTex"];
        go.AddComponent(new SphereColliderComponent(1f, false));
        go.AddComponent(new DebugMoveComponent());
        go.GetComponent<DebugMoveComponent>().move = false;
        go.AddComponent(new Follower(go, 2f));
        return go;
    }

    /// <summary>
    /// Creates a simple object with a given name and position that moves.<p> Default model and textures are "deimos"</p>
    /// </summary>
    /// <param name="nameOfObject">Name of the <see cref="GameObject"/></param>
    /// <param name="x">X position in the world</param>
    /// <param name="z">Z position in the world</param>
    /// <returns>new <see cref="GameObject"/> that moves</returns>
    private GameObject CreateMovableObject(string nameOfObject, float x, float z)
    {
        GameObject go = new GameObject(nameOfObject);
        go.transform.position = new Vector3(x, 0, z);
        Vector3 pos = go.transform.position;
        pos.Y = environmentObject.GetHeight(pos);
        go.transform.position = pos;
        
        go.model = loadedModels["deimos"];
        go.texture = loadedTextures["deimos"];
        go.AddComponent(new SphereColliderComponent(1f, false));
        go.AddComponent(new DebugMoveComponent());
        go.GetComponent<DebugMoveComponent>().move = false;
        return go;
    }

    /// <summary>
    /// Creates a simple object with a given name and position that can follow another object, WHICH SHOULD BE SET LATER.<p> Default model and textures are "deimos"</p>
    /// </summary>
    /// <param name="nameOfObject">Name of the <see cref="GameObject"/></param>
    /// <param name="x">X position in the world</param>
    /// <param name="z">Z position in the world</param>
    /// <returns>new <see cref="GameObject"/> that follows another object, WHICH SHOULD BE SET LATER</returns>
    private GameObject CreateFollowingObject(string nameOfObject, float x, float z)
    {
        GameObject go = new GameObject(nameOfObject);
        go.transform.position = new Vector3(x, 0, z);
        Vector3 pos = go.transform.position;
        pos.Y = environmentObject.GetHeight(pos);
        go.transform.position = pos;
        go.transform.scale = new Vector3(0.75f);
        go.model = loadedModels["deimos"];
        go.texture = loadedTextures["deimos"];
        go.AddComponent(new SphereColliderComponent(1f, false));
        go.AddComponent(new DebugMoveComponent());
        //go.GetComponent<DebugMoveComponent>().move = false;
        go.AddComponent(new Follower(go, 2f));
        return go;
    }

    /// <summary>
    /// Creates a simple object with a given name and position that just stands still.<p> Default model and textures are "deimos"</p>
    /// </summary>
    /// <param name="nameOfObject">Name of the <see cref="GameObject"/></param>
    /// <param name="x">X position in the world</param>
    /// <param name="z">Z position in the world</param>
    /// <returns>new <see cref="GameObject"/> that doesn't move</returns>
    private GameObject CreateStaticObject(string nameOfObject, float x, float z)
    {
        GameObject go = new GameObject(nameOfObject);
        go.model = loadedModels["deimos"];
        go.texture = loadedTextures["deimos"];
        go.transform.position = new Vector3(x, 0, z);
        Vector3 pos = go.transform.position;
        pos.Y = environmentObject.GetHeight(pos);
        go.transform.position = pos;
        go.AddComponent(new SphereColliderComponent(1f, true));
        return go;
    }
    
    private float SquaredDistanceBetweenEnemyAndPlayer()
    {
        Vector3 dist = enemy.transform.position - gab.transform.position;
        dist.Y = 0;
        return dist.LengthSquared();
    }

    private bool EnemyReachedPlayer()
    {
        return SquaredDistanceBetweenEnemyAndPlayer() <= (enemy.GetComponent<Follower>().SocialDistance *
            enemy.GetComponent<Follower>().SocialDistance);
    }
}
