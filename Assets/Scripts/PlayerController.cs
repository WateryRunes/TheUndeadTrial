using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private float _MOVEMENT_MULTIPLIER_ = 3f;
    private float _SENSITIVITY_ = 2f;
    private float _GRAVITY_ = 9.8f;
    public int health {private set ; get;}
    private GameObject objLookedAt = null;
    private CharacterController charController;
    public Camera camera;
    private GameController gameController;
    private bool charged = true;
    private bool rangedCharged = true;
    private Animator animationController;
    public GameObject Fireball;
    public GameObject rangedAttackPos;
    public bool paused = false;
    public bool footstepPlaying = false;
    public AudioSource footstepSound;
    public AudioSource hitSound;
    public AudioSource missSound;
    public AudioSource damageTakenSound;

    // Start is called before the first frame update
    void Start()
    {
        health = 75;
        camera = gameObject.transform.GetChild(0).GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        charController = gameObject.GetComponent<CharacterController>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        animationController = GameObject.FindGameObjectWithTag("Arms").GetComponent<Animator>();
    }

    private void OnLevelWasLoaded(int level)
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        animationController = GameObject.FindGameObjectWithTag("Arms").GetComponent<Animator>();
        bool attacking = false;
        bool rangedAttacking = false;
        animationController.SetBool("attacking", attacking);

        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            Cursor.visible = !Cursor.visible;
            if (!gameController.optionsCollection.activeInHierarchy)
            {
                paused = !paused;
            }

            if (!paused)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1.0f;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0.0f;
            }

            gameController.togglePauseMenu();
        }


        // sprint controls
        if (Input.GetKeyDown(KeyCode.LeftShift)) { _MOVEMENT_MULTIPLIER_ = 6f; }
        if (Input.GetKeyUp(KeyCode.LeftShift)) { _MOVEMENT_MULTIPLIER_ = 3f; }

        // simple movement of player relative to camera
        Vector3 move = (transform.forward * _MOVEMENT_MULTIPLIER_ * Input.GetAxis("Vertical")) + (transform.right * _MOVEMENT_MULTIPLIER_ * Input.GetAxis("Horizontal"));
        charController.Move(move * Time.deltaTime);

        Debug.Log(move + " " + footstepPlaying);
        if(move != Vector3.zero && !footstepPlaying)
        {
            footstepPlaying = true;
            footstepSound.Play();
        }
        else if (move ==  Vector3.zero)
        {
            footstepPlaying = false;
            footstepSound.Stop();
        }

        // rotation of player on x-axis and camera on y-axis depending on mouse movement
        float mouseX = Input.GetAxis("Mouse X") * _SENSITIVITY_;
        float mouseY = Input.GetAxis("Mouse Y") * _SENSITIVITY_;

        if (!paused)
        {
            transform.eulerAngles += new Vector3(0, mouseX, 0);
            camera.transform.eulerAngles += new Vector3(-mouseY, 0, 0);

            // clamp camera rotation to 80 degrees above/below horizon - DOES NOT WORK
            // Debug.Log(camera.transform.rotation.x);
            if (camera.transform.rotation.x > 0.8f)
            {
                // Debug.Log("here");
                camera.transform.rotation = Quaternion.Euler(new Vector3(0.8f, camera.transform.rotation.y, camera.transform.rotation.z));
            }
        }    

        // applying gravity to keep the player on the ground
        if (!charController.isGrounded)
        {
            charController.Move(new Vector3(0, -_GRAVITY_ * Time.deltaTime, 0));
        }

        // raycast to see if looking at pickup
        Ray lookingRay = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit lookingRayData;
        Debug.DrawRay(lookingRay.origin, lookingRay.direction, new Color(255,0,0));
        Physics.Raycast(lookingRay, out lookingRayData, 3);
        if(lookingRayData.collider != null && lookingRayData.collider.gameObject.tag == "PickUp")
        {
            objLookedAt = lookingRayData.collider.gameObject;
        }
        else
        {
            objLookedAt = null;
        }

        // checking for pick up of consumable and doing its effect
        if (objLookedAt != null && Input.GetKeyDown(KeyCode.E))
        {
            pickUpItem(objLookedAt);
            objLookedAt.GetComponent<MeshRenderer>().enabled = false; // make invisible while still allowing sound to be played in pickUpItem();
            Destroy(objLookedAt, 2f); // destroy after sound finished
        }

        if (Input.GetMouseButton(0))
        {
            attacking = true;

            if(attacking && charged)
            {
                animationController.SetBool("attacking", attacking);

                Ray attackingRay = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit attackingRayData;
                Debug.DrawRay(attackingRay.origin, attackingRay.direction, new Color(0, 255, 0));
                Physics.Raycast(attackingRay, out attackingRayData, 3);
                if (attackingRayData.collider != null)
                {
                    
                    if (attackingRayData.collider.gameObject.tag == "MeleeEnemy")
                    {
                        StartCoroutine(attackMeleeEnemy(attackingRayData.collider.gameObject));
                    }
                    else if (attackingRayData.collider.gameObject.tag == "RangedEnemy")
                    {
                        StartCoroutine(attackRangedEnemy(attackingRayData.collider.gameObject));
                    }
                    
                }
                else if (attackingRayData.collider == null)
                {
                    missSound.Play();
                }

                charged = false;
                StartCoroutine(ResetCharged());
            }   
        }

        if (Input.GetMouseButton(1))
        {
            rangedAttacking = true;

            if (rangedAttacking && rangedCharged)
            {
                Instantiate(Fireball, rangedAttackPos.transform.position, rangedAttackPos.transform.rotation);

                rangedCharged = false;
                StartCoroutine(ResetRangedCharged());
            }
        }

        IEnumerator ResetCharged()
        {
            yield return new WaitForSeconds(1.0f);
            charged = true;
        }

        IEnumerator ResetRangedCharged()
        {
            yield return new WaitForSeconds(6.0f);
            rangedCharged = true;
        }
    }

    IEnumerator attackMeleeEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(0.5f);
        hitSound.Play();
        enemy.GetComponent<MeleeEnemyController>().changeHealth((int)Mathf.Round(-25 * gameController.dmgMulti));
    }

    IEnumerator attackRangedEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(0.5f);
        hitSound.Play();
        enemy.GetComponent<RangedEnemyController>().changeHealth(-25);
    }

    // got to be a better way to differentiate between consumables without giving them all scripts (still not sure how to access these well) or checking for names
    public void pickUpItem(GameObject item)
    {
        if(item.name == "SmallHealth")
        {
            updateHealth(25);
        }
        else if (item.name == "MediumHealth")
        {
            updateHealth(50);
        }
        else if (item.name == "LargeHealth")
        {
            updateHealth(75);
        }
        else if (item.name == "MagicCarrot")
        {
            gameController.dmgMulti = 1.5f;
        }
        else if (item.name == "Trophy")
        {
            StartCoroutine(Win());
        }

        if (item.GetComponent<AudioSource>() != null)
        {
            item.GetComponent<AudioSource>().Play();
        }

        // potion drink sound - https://www.youtube.com/watch?v=UW7tFLAn4gI&ab_channel=FreeSoundEffects
    }

    public void hit(int change)
    {
        updateHealth(change);
    }

    public void updateHealth(int change, bool textUpdate = true)
    {
        if(change < 0)
        {
            damageTakenSound.Play();
        }
        if(health + change > 100)
        {
            change = 100 - health;
        }
        if (health + change < 0)
        {
            change = -health;
        }
        health += change;
        if (textUpdate && change != 0)
        {
            gameController.updateHealthText(health, change);
        }
        else if (!textUpdate)
        {
            gameController.updateHealthText(health, change, false);
        }
        if(health == 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        paused = true;
        yield return new WaitForSeconds(1);
        gameController.deathScreen();
        yield return new WaitForSeconds(2);
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        gameController.endGame();
    }

    IEnumerator Win()
    {
        paused = true;
        gameController.winScreen();
        yield return new WaitForSeconds(2);
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        gameController.endGame();
    }
}
