using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Render : MonoBehaviour
{
    public int gameFrame = 0;
    private Ground ground;
    public SaveStateManager saveStateManager;
    public GameDataTransport dataTransport;
    public AttackData attackData;
    public StartGameAnim startGameAnim;

    public Camera mainCamera;

    public GameObject displayPrefab, canvas, shieldPrefab, projectilePrefab, blastZonePrefab, boxRendererPrefab, pauseMenu;

    public GameObject[] effectPrefabs;

    [Header("Hurtbox Settings")]
    public Material lineMaterial;
    public float lineThickness;
    public Gradient p1Gradient;
    public Gradient p2Gradient;
    public Gradient groundBoxGradient;

    [Header("Game Options")]
    public float topBlastZoneSize;
    public float sideBlastZoneSize;
    public float bottomBlastZoneSize;
    public byte maxProjectiles = 10;
    public Vector2 borderOffset;
    public int maxSaveFrameSize = 9000;

    [Header("Gameplay")]
    public int playerAmount = 2;
    private MapData[] mapData;
    [HideInInspector] public CharacterData[] charStats;
    public List<Player> players = new();
    [HideInInspector] public List<Hitbox> hitboxes = new();
    public List<Projectile> projectiles = new();
    [HideInInspector]
    public int rollbackFrames;
    public Dictionary<int, PlayerInput> opponentInputs = new();
    public byte localPlayerID;

    public bool showBoxes = true;
    private bool _paused = false;
    private bool stateRecentlyLoaded = false;

    [HideInInspector] public Dictionary<int, SaveState> savedFrames = new Dictionary<int, SaveState>();
    [HideInInspector] public List<int> playerStates = new();
    [HideInInspector] public int timeLeft;
    public bool Paused
    {
        get { return _paused; }
        set
        {
            _paused = value;
            if (_paused)
                pauseMenu.SetActive(true);
            else
                pauseMenu.SetActive(false);
        }
    }

    [HideInInspector]
    public GroundBox[] groundBoxes;

    [HideInInspector]
    public int mapId;

    private void Awake()
    {
        if (GameManager.instance == null)
        {
            Application.Quit();
            return;
        }
        mapData = GameManager.instance.maps;
        charStats = GameManager.instance.characters;
        if (GameManager.instance.mapID >= mapData.Length)
            mapId = Random.Range(0, mapData.Length);
        else
            mapId = GameManager.instance.mapID;
        groundBoxes = mapData[mapId].groundBoxes;

        ground = GameObject.Find("Ground").GetComponent<Ground>();

        for (byte i = 0; i < playerAmount; i++)
        {
            players.Add(new Player(i, 
                (GameManager.instance.playerTypes[i] != GameManager.PlayerTypes.Local && GameManager.instance.playerTypes[i] != GameManager.PlayerTypes.Online) ? (byte)GameManager.instance.gameSettings.cpuChar : GameManager.instance.playerCharacterIds[i], 
                GameManager.instance.playerTypes[i]));
            players[i].Init(true);

            GameObject displayer = Instantiate(displayPrefab, canvas.transform);
            displayer.GetComponent<CharacterDisplayer>().playerIndex = i;

            GameObject shield = Instantiate(shieldPrefab);
            shield.GetComponent<Shield>().player = players[i];
            if (GameManager.instance.gameType == GameManager.GameTypes.Online && GameManager.instance.playerTypes[i] == GameManager.PlayerTypes.Local) localPlayerID = i;
        }

        for (byte i = 0; i < maxProjectiles; i++)
        {
            projectiles.Add(new Projectile(i));
        }

        pauseMenu.SetActive(false);
        timeLeft = GameManager.instance.gameSettings.time * 1800;
        rollbackFrames = 0;
    }

    private void Update()
    {
        if (Application.targetFrameRate > 35) // fix with webgl builds
            if (Time.frameCount % 2 == 1) return;

        if (Input.GetKeyDown(KeyCode.Space) && !playerStates.Contains(24) && GameManager.instance.gameType != GameManager.GameTypes.Online)
            if (Paused)
            {
                int cursorId = pauseMenu.GetComponent<PauseMenu>().cursorId;
                if (cursorId == 0)
                    Paused = false;
                else if (cursorId == 1)
                {
                    showBoxes = !showBoxes;
                    DrawBoxes();
                }
                else if (cursorId == 2)
                    SceneManager.LoadScene("Title");
            }
            else
                Paused = true;

        if (Paused)
            return;

        if (!(playerStates.Contains(24) && gameFrame % 2 == 1) || timeLeft == 1)
            RunFrame();
        else
            gameFrame++;
    }

    // Main game loop
    public void RunFrame()
    {
        if (!stateRecentlyLoaded)
        {
            gameFrame++;
            FilterHitboxes();
        }
        // Render players
        for (int i = 0; i < players.Count; i++)
        {
            players[i].Render();
        }
        // Render Projectiles
        for (int i = 0; i < projectiles.Count; i++)
        {
            projectiles[i].Render();
        }

        DrawBoxes();
        SetCamera();

        if (timeLeft != 0)
            timeLeft--;

        // check for game winner
        playerStates.Clear();
        for (int i = 0; i < players.Count; i++) playerStates.Add(players[i].state);
        if (playerStates.Contains(24) || timeLeft == 1)
        {
            bool temp = false;
            if (playerStates.Contains(24)) if (players[playerStates.IndexOf(24)].frame < 1) temp = true;
            if (temp || timeLeft == 1)
            {
                GameManager.instance.gameWinner = GameUtils.GetWinner(players[0].stocks, players[0].damage, players[1].stocks, players[1].damage);
                startGameAnim.ShowGameText();
                if (timeLeft == 1)
                {
                    GameManager.instance.playerStore = new(players);
                    SceneManager.LoadScene("Win Screen");
                    return;
                }

            }
            if (players[playerStates.IndexOf(24)].frame > 30)
            {
                GameManager.instance.playerStore = new(players);
                SceneManager.LoadScene("Win Screen");
            }
        }
        // countdown animation
        if (gameFrame < 92 && GameManager.instance.gameType != GameManager.GameTypes.Training)
        {
            startGameAnim.gameObject.SetActive(true);
            startGameAnim.Countdown(gameFrame);
            timeLeft++;
        }
        else if (GameManager.instance.gameType == GameManager.GameTypes.Training)
            startGameAnim.gameObject.SetActive(false);

        if (rollbackFrames > 0)
        {
            rollbackFrames--;
        }
        else if (GameManager.instance.gameType == GameManager.GameTypes.Online)
        {
            dataTransport.SendMessage(new TransportData
            {
                gameFrame = gameFrame,
                playerId = localPlayerID,
                playerInput = players[localPlayerID].inputs.ToPlayerInput()
            }) ;
        }

        if (stateRecentlyLoaded)
        {
            stateRecentlyLoaded = false;
            return;
        }
        // save state
        savedFrames.Add(gameFrame, saveStateManager.SaveToState());

        if (savedFrames.Count > maxSaveFrameSize)
            savedFrames.Remove(savedFrames.Count - (maxSaveFrameSize + 1));

    }
    //render player and ground hitboxes, hitboxes are drawn automatically
    public void DrawBoxes()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].Draw();
        }

        ground.DrawGroundBoxes();
    }

    public void SetCamera()
    {
        var count = 0;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].state != 24) count++;
        }
        if (count == 0) return;

        float targetScale;
        if ((players[0].state == 21 && gameFrame < 60) || count == 1)
        {
            targetScale = 15f;
        }
        else
        {
            List<Vector2> positions = new();
            foreach (Player player in players)
            {
                positions.Add(player.pos);
            }
            positions = positions.OrderBy(v => v.x).ToList();
            targetScale = Vector3.Distance(positions[0], positions[positions.Count-1])*1.3f;
            targetScale = Mathf.Clamp(targetScale, 20f, 35f);
        }


        mainCamera.orthographicSize = ((mainCamera.orthographicSize * 7) + (targetScale / mainCamera.aspect)) / 8;

        Vector3 center = new();
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].state != 24)
            {
                center += new Vector3(players[i].pos.x, players[i].pos.y);
            }
        }
        center /= count;
        if (players[0].state == 21) // Countdown camera animation
        {
            if (gameFrame < 30) center = players[0].pos;
            else if (gameFrame < 60) center = players[1].pos;
        }
        Vector3 newPos = ((mainCamera.transform.position * 4) + center) / 5;
        float x = Mathf.Clamp(newPos.x, (-(sideBlastZoneSize - borderOffset.x)) + (mainCamera.orthographicSize * mainCamera.aspect), sideBlastZoneSize - borderOffset.x - (mainCamera.orthographicSize * mainCamera.aspect));
        float y = Mathf.Clamp(newPos.y, bottomBlastZoneSize + borderOffset.y + mainCamera.orthographicSize, topBlastZoneSize - (borderOffset.y - 9f) - mainCamera.orthographicSize);

        mainCamera.transform.position = new Vector3(x, y);
    }

    public void CreatePlayerHitbox(int p, Vector2 offset, Vector2 size, Vector2 mult, int damage, byte type = default)
    {
        Hitbox hitbox = new(players[p], offset, size, mult, damage, type);
        hitboxes.Add(hitbox);
    }

    public void CreateProjectileHitbox(int id, float offsetX, float offsetY, float sizeX, float sizeY, float multX, float multY, int damage, byte type = default)
    {
        Hitbox hitbox = new(projectiles[id], new Vector2(offsetX, offsetY), new Vector2(sizeX, sizeY), new Vector2(multX, multY), damage, type);
        hitboxes.Add(hitbox);
    }
   
    //removes inactive hitboxes
    public void FilterHitboxes()
    {
        for (int i = 0; i < hitboxes.Count; i++)
        {
            if (!hitboxes[i].fresh)
            {
                hitboxes.RemoveAt(i);
                i--;
            }
            else
            {
                hitboxes[i].fresh = false;
                hitboxes[i].SetInactive();
            }
        }
    }

    public void CreateProjectile(float offsetX, float offsetY, string type, byte creator)
    {
        Player player = players[creator];
        for (int i = 0; i < projectiles.Count; i++)
        {
            if (projectiles[i].type == 0)
            {
                projectiles[i].pos = new Vector2(player.pos.x + (offsetX * player.dir), player.pos.y + offsetY);
                projectiles[i].type = GameUtils.projNames[type];
                projectiles[i].creator = creator;
                projectiles[i].dir = player.dir;
                projectiles[i].frame = 0;

                //GameObject projectileRenderer = Instantiate(projectilePrefab, transform);
                //projectileRenderer.GetComponent<ProjectileRenderer>().myProjectile = projectiles[i];

                return;
            }
        }
    }

    public void CreateBlastZoneEffect(Vector2 pos)
    {
        if (!Paused)
        {
            GameObject blastZone = Instantiate(blastZonePrefab, transform);
            blastZone.GetComponent<BlastZoneEffect>().deathPos = pos;
        }
    }

    // loads a save state by replacing data
    public void ReplaceRenderLists(SaveState saveState, bool rollback = false)
    {

        for (int i = 0; i < saveState.playerSaveData.Length; i++)
        {
            players[i].LoadStateData(saveState.playerSaveData[i]);
        }

        for (int i = 0; i < saveState.projectiles.Length; i++)
        {
            projectiles[i].LoadStateData(saveState.projectiles[i]);
        }

        for (int i = 0; i < hitboxes.Count; i++)
        {
            hitboxes[i].SetInactive();
        }
        hitboxes.Clear();

        for (int i = 0; i < saveState.hitboxes.Length; i++)
        {
            hitboxes.Add(saveState.hitboxes[i]);
        }

        stateRecentlyLoaded = true; // Prevents FilterHitboxes() from being called next frame

        for (int i = gameFrame; i > saveState.gameFrame; i--)
        {
            savedFrames.Remove(i);
            if (rollback) rollbackFrames++;
        }

        gameFrame = saveState.gameFrame;
    }
}
