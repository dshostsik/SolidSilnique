using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SolidSilnique.Core;
using SolidSilnique.Core.ArtificialIntelligence;
using SolidSilnique.Core.Components;
using SolidSilnique.ProcderuralFoliage;
using System.Collections.Generic;
using System.Threading.Tasks;
using GUIRESOURCES;
using Microsoft.Xna.Framework.Input;
using SolidSilnique.Core.Animation;
using SolidSilnique.MonoAL;
using static BossRhythymUI;
using System.Linq;


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
    BossRhythymUI bossRhythym = new BossRhythymUI();
    BossRhythymUI bossRhythym2 = new BossRhythymUI();


    private AudioPlayer hitSounds;
    SpriteBatch spriteBatch = new SpriteBatch(EngineManager.graphics);
    public GUI rhythymGui;

    //----Main-menu-ahh-setup----
    private bool inMainMenu = true;

    public GUI mainMenuGui;

    //---------------------------
    ContentManager content;

    // let's assume it as a player object so far
    private GameObject gab;
    private GameObject enemy;
    private int _enemyHP = 100;
    private bool _playerInsideEnemyFOV = false;
    private bool _songWasPlaying = false;
    private double _lastAudioStopTime;

    private bool firstFrame = true;

    private FightGrade grade;
    
    public ProceduralTest()
    {
    }

    public override void LoadContent(ContentManager Content)
    {
        //loadedModels.Add("drzewo", Content.Load<Model>("drzewo2"));
        loadedModels.Add("deimos", Content.Load<Model>("deimos"));
        //loadedModels.Add("plane", Content.Load<Model>("plane"));
        loadedModels.Add("cube", Content.Load<Model>("cube"));
        //loadedModels.Add("cone", Content.Load<Model>("cone"));
        loadedModels.Add("sphere", Content.Load<Model>("sphere"));
        loadedModels.Add("levelTest", Content.Load<Model>("level_ground"));
        loadedModels.Add("brzewno1/brzewno", Content.Load<Model>("brzewno1/brzewno"));
        loadedModels.Add("brzewno3/brzewno", Content.Load<Model>("brzewno3/brzewno3"));
        //loadedModels.Add("drzewo/drzewo", Content.Load<Model>("drzewo/drzewo"));
        loadedModels.Add("trent", Content.Load<Model>("trent"));

        loadedTextures.Add("deimos", Content.Load<Texture2D>("Map1/layer1/diffuse"));
        //loadedTextures.Add("testTex", Content.Load<Texture2D>("testTex"));
        loadedModels.Add("dodik", Content.Load<Model>("dodik"));

        //loadedTextures.Add("deimos", Content.Load<Texture2D>("deimos_texture"));
        //loadedTextures.Add("testTex", Content.Load<Texture2D>("testTex"));
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

        loadedTextures.Add("trent", Content.Load<Texture2D>("trent_fire/low_material_Base_color"));
        loadedTextures.Add("trent/normal", Content.Load<Texture2D>("trent_fire/low_material_Normal_DirectX"));

        loadedTextures.Add("dodik_texture", Content.Load<Texture2D>("Textures/purple"));
        //loadedTextures.Add("trent/ao", Content.Load<Texture2D>("trent_fire/PM3D_Cylinder3D_10_Mixed_AO"));
        //loadedTextures.Add("trent/roughness", Content.Load<Texture2D>("trent_fire/PM3D_Cylinder3D_10_Coat_roughness"));

        models.Add(Content.Load<Model>("pModels/Rock1")); textures.Add(loadedTextures["deimos"]);
		//models.Add(Content.Load<Model>("pModels/Branch")); textures.Add(loadedTextures["deimos"]);
		models.Add(Content.Load<Model>("pModels/BushBig")); textures.Add(loadedTextures["leafTex"]);
		models.Add(Content.Load<Model>("pModels/BushBig")); textures.Add(loadedTextures["leafTex"]);
		models.Add(Content.Load<Model>("pModels/BushBig")); textures.Add(loadedTextures["leafTex"]);
		models.Add(Content.Load<Model>("pModels/BushBig")); textures.Add(loadedTextures["leafTex"]);
		models.Add(Content.Load<Model>("pModels/BushBig")); textures.Add(loadedTextures["leafTex"]);
		models.Add(Content.Load<Model>("pModels/BushBig")); textures.Add(loadedTextures["leafTex"]);
		models.Add(Content.Load<Model>("pModels/BushBig")); textures.Add(loadedTextures["leafTex"]);
		models.Add(Content.Load<Model>("pModels/BushBig")); textures.Add(loadedTextures["leafTex"]);
		models.Add(Content.Load<Model>("pModels/BushSmall")); textures.Add(loadedTextures["deimos"]);
		models.Add(Content.Load<Model>("pModels/BushSmall")); textures.Add(loadedTextures["deimos"]);
		models.Add(Content.Load<Model>("pModels/BushSmall")); textures.Add(loadedTextures["deimos"]);
		//models.Add(Content.Load<Model>("pModels/Log")); textures.Add(loadedTextures["deimos"]);
		models.Add(Content.Load<Model>("pModels/Stump")); textures.Add(loadedTextures["deimos"]);

		//models.Add(Content.Load<Model>("brzewno1/brzewno"));
		//models.Add(Content.Load<Model>("brzewno3/brzewno3"));
		//textures.Add(loadedTextures["leafTex"]);
        //textures.Add(loadedTextures["deimos"]);

        treeModels.Add(Content.Load<Model>("pModels/tree1"));
        treeModels.Add(Content.Load<Model>("pModels/Tree2"));
        treeTextures.Add(Content.Load<Texture2D>("Textures/tree1_diffuse"));
        treeTextures.Add(Content.Load<Texture2D>("Textures/tree2_diffuse"));
        //treeTextures.Add(Content.Load<Texture2D>("Textures/gab_tex"));
        loadedModels.Add("tower", Content.Load<Model>("Models/tower"));
        loadedModels.Add("eMonster1", Content.Load<Model>("Models/monstr"));
        loadedTextures.Add("F", Content.Load<Texture2D>("Grades/F"));
        loadedTextures.Add("D", Content.Load<Texture2D>("Grades/D"));
        loadedTextures.Add("C", Content.Load<Texture2D>("Grades/C"));
        loadedTextures.Add("B", Content.Load<Texture2D>("Grades/B"));
        loadedTextures.Add("A", Content.Load<Texture2D>("Grades/A"));
        loadedTextures.Add("S", Content.Load<Texture2D>("Grades/S"));
        

        content = Content;

        
    }

    public override void Setup()
    {
	    /*
	    string[] sounds = new string[4];
	    sounds[0] = "Content/Sounds/drum-hitclap.wav";
	    sounds[1] = "Content/Sounds/drum-hitfinish.wav";
	    sounds[2] = "Content/Sounds/drum-hitnormal.wav";
	    sounds[3] = "Content/Sounds/drum-hitnormalh.wav";
	    hitSounds = new AudioPlayer(sounds, PositionalHelper.GetInstance(), 1f, 1f);
	    */
        environmentObject = new EnvironmentObject();
        environmentObject.Generate("Map1", content, 3, 60, 3, 8);

        _lastAudioStopTime = 0f;
        
        ProceduralGrass newProc =
            new ProceduralGrass(models, textures, treeModels, treeTextures, content, environmentObject);
        Task task1 = Task.Run(() => newProc.precomputeNoise());

        GameObject go = new GameObject("Camera");
        go.transform.position = new Vector3(250, 3f, 250);
        CameraComponent cam = new CameraComponent();
        cam.SetMain();
        go.AddComponent(cam);
        go.AddComponent(new TPPCameraComponent());
        this.AddChild(go);

        
        Task.WhenAll(task1).Wait(); //:O

        newProc.GenerateObjects();
        List<GameObject> goList = newProc.createdObjects;

        for (int a = 0; a < goList.Count; a++)
        {
            this.AddChild(goList[a]);
        }


        gab = new GameObject("gab");
		GameObject gabV = new GameObject("gabVisual");
        gab.AddChild(gabV);


		gab.transform.position = new Vector3(180, 15, 730);

        gab.transform.scale = new Vector3(1f);
        gabV.model = loadedModels["cube"];
        gabV.texture = loadedTextures["gabTex"];
        //gab.normalMap = loadedTextures["gabNo"];
        //gab.roughnessMap = loadedTextures["gabRo"];
        //gab.aoMap = loadedTextures["gabAo"];
        gab.AddComponent(new DebugMoveComponent());
        gab.AddComponent(new SphereColliderComponent(1));
        gabV.AddComponent(new NoteResponseComponent());
        this.AddChild(gab);


        GameObject TPcam = new GameObject("cam");
        TPcam.AddComponent(new TPPCameraComponent());
        TPcam.transform.position = new Vector3(0, 1.5f, 0);
        gab.AddChild(TPcam);

        GameObject TPcamCam = new GameObject("camcam");
        var tpcCamComp = new CameraComponent();
        TPcamCam.AddComponent(tpcCamComp);


        TPcamCam.transform.position = new Vector3(0, 0, 10);
        this.TPCamera = new Camera(tpcCamComp);
        TPcam.AddChild(TPcamCam);


        GameObject gabFur = new GameObject("gabFur");
        gabFur.transform.position = new Vector3(0f, 0f, 0f);
        gabFur.transform.scale = new Vector3(1f);
        gabFur.model = loadedModels["dodik"];
        gabFur.texture = loadedTextures["dodik_texture"];
       gabV.AddChild(gabFur);
        
        GameObject eye1 = new GameObject("eye1");
        eye1.transform.position = new Vector3(-0.25f * 2, 0.209f, -0.495f * 2);
        eye1.transform.scale = new Vector3(0.4f);
        eye1.model = loadedModels["sphere"];
        eye1.texture = loadedTextures["eye"];
        gabV.AddChild(eye1);

			GameObject pupil1 = new GameObject("pupil1");
				pupil1.transform.position = new Vector3(0, 0, -0.495f * 2);
				pupil1.transform.scale = new Vector3(0.4f,0.4f,0.2f);
				pupil1.model = loadedModels["sphere"];
				pupil1.texture = loadedTextures["simpleBlack"];
				eye1.AddChild(pupil1);

		GameObject brow1 = new GameObject("brow1");
		brow1.transform.position = new Vector3(-0.25f * 2, 0.5f, -0.495f * 2);
		brow1.transform.scale = new Vector3(0.45f, 0.2f,0.4f);
		brow1.transform.rotation = new Vector3(0f,0,-20f);
		brow1.model = loadedModels["cube"];
		brow1.texture = loadedTextures["simpleBlack"];
		gabV.AddChild(brow1);

		GameObject eye2 = new GameObject("eye2");
			eye2.transform.position = new Vector3(0.25f*2, 0.209f, -0.495f * 2);
			eye2.transform.scale = new Vector3(0.4f);
			eye2.model = loadedModels["sphere"];
			eye2.texture = loadedTextures["eye"];
			gabV.AddChild(eye2);

		GameObject pupil2 = new GameObject("pupil1");
		pupil2.transform.position = new Vector3(0, 0, -0.495f * 2);
		pupil2.transform.scale = new Vector3(0.4f, 0.4f, 0.2f);
		pupil2.model = loadedModels["sphere"];
		pupil2.texture = loadedTextures["simpleBlack"];
		eye2.AddChild(pupil2);

		GameObject brow2 = new GameObject("brow1");
		brow2.transform.position = new Vector3(0.25f * 2, 0.5f, -0.495f * 2);
		brow2.transform.scale = new Vector3(0.45f, 0.2f, 0.4f);
		brow2.transform.rotation = new Vector3(0f, 0, 20f);
		brow2.model = loadedModels["cube"];
		brow2.texture = loadedTextures["simpleBlack"];
		gabV.AddChild(brow2);


        GameObject Tower = new GameObject("tower");
        Tower.transform.position = new Vector3(512, 80, 0);
        Tower.transform.scale = new Vector3(20, 80, 20);
        Tower.transform.rotation = new Vector3(0f, 0, 0f);
        Tower.model = loadedModels["tower"];
        Tower.texture = loadedTextures["eye"];
        var clip1 = new AnimationClip();
        clip1.PositionCurve.AddKey(new Keyframe<Vector3>(0f, new Vector3(512, 80, 0)));
        clip1.PositionCurve.AddKey(new Keyframe<Vector3>(2f, new Vector3(512, 80, 0)));
        // scale: pulse 1→2→1
        clip1.ScaleCurve.AddKey(new Keyframe<Vector3>(0f, new Vector3(20, 60, 20)));
        clip1.ScaleCurve.AddKey(new Keyframe<Vector3>(1f, new Vector3(20, 90, 20)));
        clip1.ScaleCurve.AddKey(new Keyframe<Vector3>(2f, new Vector3(20, 60, 20)));

        var animator1 = new AnimatorComponent(clip1, loop: true);

        Tower.AddComponent(animator1);
        animator1.Play();
        this.AddChild(Tower);


        /*var cube = new GameObject("AnimatedCube");
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
        //animator.Play();

        // add to scene
        this.AddChild(cube);*/


        GameObject prevGeb = gab;
			/*for (int i = 0; i < 3; i++)
			{
				GameObject gogus = CreateGebus(new Vector3(250 + i*20, 80, 250 + i*20));
				gogus.GetComponent<Follower>().Target = gab;
				if (i == 0) gogus.GetComponent<Follower>().SocialDistanceMultiplier = 4.0f;
				this.AddChild(gogus);
				//prevGeb = gogus;
			}*/

        GameObject gigus = CreateGebus(new Vector3(165, 15, 600),1,"Easy","wav");
		gigus.GetComponent<Follower>().Target = gab;
		this.AddChild(gigus);

		gigus = CreateGebus(new Vector3(150, 15, 550),2,"Easy","wav");
		gigus.GetComponent<Follower>().Target = gab;
		this.AddChild(gigus);

		gigus = CreateGebus(new Vector3(150, 15, 500),3,"Easy","mp3");
		gigus.GetComponent<Follower>().Target = gab;
		this.AddChild(gigus);

		gigus = CreateGebus(new Vector3(250, 15, 650), 1, "Hard", "wav");
		gigus.GetComponent<Follower>().Target = gab;
		this.AddChild(gigus);

		gigus = CreateGebus(new Vector3(300, 15, 650), 2, "Hard", "wav");
		gigus.GetComponent<Follower>().Target = gab;
		this.AddChild(gigus);

		gigus = CreateGebus(new Vector3(400, 15, 650), 3, "Hard", "mp3");
		gigus.GetComponent<Follower>().Target = gab;
		this.AddChild(gigus);


		//gigus.albedo = Color.Red;



		GameObject ziutek = CreateMovableObject("ziutek", 200, 200);
        ziutek.GetComponent<SphereColliderComponent>().boundingSphere.Radius = 2.0f;
        //ziutek.GetComponent<Follower>().Target = gab;
        this.AddChild(ziutek);

        mainMenuGui = new GUI("Content/MainMenuUI/menu.xml", content);
        rhythymGui = new GUI("Content/RhythymGui.xml", content);
        //EngineManager.currentGui = rhythymGui;

        // Check if we're playing or nah
        EngineManager.darkenTheScene = inMainMenu ? 1 : 0;
        EngineManager.currentGui = inMainMenu ? mainMenuGui : rhythymGui;
        EngineManager.mouseFree = inMainMenu;
        EngineManager.mouseVisible = inMainMenu;

        /*enemy = new GameObject("euzebiusz wiercibok");
        enemy.AddComponent(new Follower(enemy, 5.0f));
        enemy.GetComponent<Follower>().gameObject = enemy;
        enemy.model = loadedModels["sphere"];
        enemy.texture = loadedTextures["deimos"];
        this.AddChild(enemy);*/

        this.AddChild(go);


        GameObject boss = new GameObject("boss");
		boss.transform.position = new Vector3(512, 0, 50);
        boss.albedo = new Color(1, 0.2f, 1);
		boss.model = loadedModels["trent"];
        boss.texture = loadedTextures["trent"];
        boss.AddComponent(new SphereColliderComponent(8f));
        this.AddChild(boss);

        //Overlord :)
        GameObject overlord = new GameObject("Overlord");
        overlord.AddComponent(new OverlordComponent());
        overlord.GetComponent<OverlordComponent>().currentGui = rhythymGui;
        this.AddChild(overlord);



        TPCamera.cameraComponent.SetMain();
        EngineManager.InputManager.gMode = true;
        //bossRhythym.hit += powiedzDupa;
        bossRhythym.hit += OnBossNoteHit;

        GameObject visual = new GameObject("GebusVisual");
        visual.model = loadedModels["sphere"];
        visual.texture = loadedTextures["gabTex"];
        visual.albedo = Color.Red;
        go.AddChild(visual);
        
        EngineManager.spritePos = new Rectangle((int)(EngineManager.windowWidth * 0.8f), (int)(EngineManager.windowHeight * 0.1f), (int)(EngineManager.windowWidth * 0.15f), (int)(EngineManager.windowWidth * 0.15f));
    }

    public override void Update()
    {
        //kState = Keyboard.GetState();

       /* if (SquaredDistanceBetweenEnemyAndPlayer() <
            enemy.GetComponent<Follower>().SocialDistance * 3.0f)
        {
            _playerInsideEnemyFOV = true;
            enemy.GetComponent<Follower>().Target = gab;
        }*/


        // Suspend the game if an Escape key was pressed
        if (Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
        {
            // Switch mode
            inMainMenu = true;
            EngineManager.mouseVisible = true;
            // Make the entire scene dark
            EngineManager.darkenTheScene = 1;
            // Switch interface
            EngineManager.currentGui = mainMenuGui;
            // Unlock the mouse
            EngineManager.mouseFree = inMainMenu;
            if (turnedOn && !_songWasPlaying)
            {
	            bossRhythym.audio.Pause();
	            _songWasPlaying = true;
            }
        }

        // Process only menu if inMainMenu is true. All updates will be suspended. Otherwise, update all entities
        if (inMainMenu)
        {
// For a debug version
#if DEBUG
            Console.WriteLine("Mouse pos in menu: " + Mouse.GetState().X + " " + Mouse.GetState().Y);
#endif
            CheckHovers();
        }
        else
        {
	        if ((Follower.enemyToFight != null && !turnedOn))
	        {
		        bossRhythym.hasEnded = false;
		        bossRhythym.Start(content, spriteBatch, Follower.enemyToFight.GetComponent<Follower>().enemyIndex, Follower.enemyToFight.GetComponent<Follower>().difficulty, Follower.enemyToFight.GetComponent<Follower>().audioExtension);
		        turnedOn = true;
		        OverlordComponent.instance.SetFight(bossRhythym, 100, gab, Follower.enemyToFight);
	        }

	        if (turnedOn)
	        {
		        bossRhythym.Update();
	        }

	        if (bossRhythym.hasEnded && turnedOn)
	        {
		        turnedOn = false;
		        Follower.enemyToFight.GetComponent<Follower>().SetFriendly();
		        Follower.enemyToFight = null;
		        OverlordComponent.instance.FinishFight(TPCamera.cameraComponent);
		        grade = OverlordComponent.instance.FinalGrade;

		        switch (grade)
		        {
			        case FightGrade.A:
				        EngineManager.gradeTexture = loadedTextures["A"];
				        break;
			        case FightGrade.B:
				        EngineManager.gradeTexture = loadedTextures["B"];
				        break;
			        case FightGrade.C:
				        EngineManager.gradeTexture = loadedTextures["C"];
				        break;
			        case FightGrade.D:
				        EngineManager.gradeTexture = loadedTextures["D"];
				        break;
			        case FightGrade.F:
				        EngineManager.gradeTexture = loadedTextures["F"];
				        break;
			        case FightGrade.S:
				        EngineManager.gradeTexture = loadedTextures["S"];
				        break;
		        }

		        EngineManager.timePoint = 0;
	        }

	        rhythymGui.progressBars[0].progress = bossRhythym.health;
	        rhythymGui.texts[0].text = bossRhythym.ReturnScoresAndAccuracy().ToString();
	        rhythymGui.texts[1].text = bossRhythym.combo.ToString();


        }
	    base.Update();
    }

    /// <summary>
    /// Function that checks if <see cref="Mouse"/> position intersects any of <see cref="mainMenuGui"/> buttons
    /// </summary>
    private void CheckHovers()
    {
        int mouseX = Mouse.GetState().X;
        int mouseY = Mouse.GetState().Y;

        // Check ONLY if mouse was pressed somewhere in menu
        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
        {
            for (int i = 0; i < mainMenuGui.buttons.Count; i++)
            {
                Button button = mainMenuGui.buttons[i];

                int x = (int)Math.Clamp(mouseX, button.positionX, button.positionX + button.width);
                int y = (int)Math.Clamp(mouseY, button.positionY, button.positionY + button.height);
                
                // if mouse actually intercepts any button, then check it's number. predefined in Content/MainMenuGUI/menu.xml
                if (mouseX == x && mouseY == y)
                {
                    switch (i)
                    {
                        // Play button
                        case 0:
                            inMainMenu = false;
                            EngineManager.darkenTheScene = 0;
                            EngineManager.currentGui = rhythymGui;
                            EngineManager.mouseFree = inMainMenu;
                            EngineManager.mouseVisible = false;
                            if (_songWasPlaying && turnedOn)
                            {
	                            bossRhythym.audio.Play();
	                            _songWasPlaying = false;
                            }
                            break;
                        // Escape button
                        case 1:
                            EngineManager.CloseGame = true;
                            break;
                        // The Settings button is not implemented. idk if we really need it
                    }
                }
            }
        }

        if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed && inMainMenu)
        {
	        inMainMenu = false;
	        EngineManager.darkenTheScene = 0;
	        EngineManager.currentGui = rhythymGui;
	        EngineManager.mouseFree = inMainMenu;
	        EngineManager.mouseVisible = false;
	        if (_songWasPlaying && turnedOn)
	        {
		        bossRhythym.audio.Play();
		        _songWasPlaying = false;
	        }
        }

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && inMainMenu)
        {
	        EngineManager.CloseGame = true;
        }
    }

    GameObject CreateGebus(Vector3 pos, int enemyIndex, string difficulty, string audioExtension)
    {
        GameObject go = new GameObject("Gebus");

        go.transform.position = pos;
        go.transform.scale = new Vector3(0.75f);
        
        go.AddComponent(new SphereColliderComponent(0.75f, false));
        go.AddComponent(new Follower(go, 2f) { 
            difficulty = difficulty, audioExtension = audioExtension, enemyIndex = enemyIndex
        });


        GameObject visual = new GameObject("GebusVisual");
		visual.model = loadedModels["sphere"];
		visual.texture = loadedTextures["gabTex"];
        visual.albedo = Color.Red;
        go.AddChild(visual);

        var clip1 = new AnimationClip();
        clip1.PositionCurve.AddKey(new Keyframe<Vector3>(0f, Vector3.Up * 0));

        clip1.PositionCurve.AddKey(new Keyframe<Vector3>(0.4f, Vector3.Up * 1));
        clip1.PositionCurve.AddKey(new Keyframe<Vector3>(0.8f, Vector3.Up * 0));

        clip1.ScaleCurve.AddKey(new Keyframe<Vector3>(0f, new Vector3(1f)));

        var animator2 = new AnimatorComponent(clip1, loop: true);

        visual.AddComponent(animator2);
        animator2.Play();

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

    private bool turnedOn = false;

    private float SquaredDistanceBetweenEnemyAndPlayer()
    {
        return 0; // Vector3.DistanceSquared(enemy.transform.position, gab.transform.position);
    }

    private bool EnemyReachedPlayer()
    {
        return false; // SquaredDistanceBetweenEnemyAndPlayer() < enemy.GetComponent<Follower>().SocialDistance *
        //enemy.GetComponent<Follower>().SocialDistance;
    }

    private void powiedzDupa(object sender, BossRhythymUI.NoteHitEventArgs args)
    {
        //Console.WriteLine(args.NoteType);
    }
    private void OnBossNoteHit(object sender, NoteHitEventArgs e)
    {
        // ensure gab has the component
        var responder = gab.children[0].GetComponent<NoteResponseComponent>();
        if (responder == null) return;

        // map button index → nod direction
        NodDirection dir = e.NoteType switch
        {
            0 => NodDirection.Forward,   // I
            2 => NodDirection.Backward,  // K
            1 => NodDirection.Left,      // J
            3 => NodDirection.Right,     // L
            _ => NodDirection.Forward
        };

        // trigger the quick nod
        responder.Trigger(dir);
    }
}