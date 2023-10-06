using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class PotionScript : MonoBehaviour
{
    public enum Type {
        Health,
        Damage,
        Speed
    }

    public enum Size {
        Small,
        Big
    }

    public Type potionType;
    public Size potionSize;
    public Material Health;
    public Material Damage;
    public Material Speed;
    public SteamVR_Input_Sources controller;
    private Transform player;
    private bool canDrink = true;
    private int drinkingTime = 0;
    private bool isDrinking = false;
    private bool isEmpty = false;

    private void Start()
    {
        player = GameObject.Find("[CameraRig]").transform.Find("Camera");
        switch (potionType)
        {
            case Type.Health:
                transform.Find("Model").Find("orb").Find("Particle").GetComponent<Renderer>().material = Health;
                break;
            case Type.Damage:
                var color = new Color(152f / 255f, 0, 1);
                var propertyBlock = new MaterialPropertyBlock();
                propertyBlock.SetColor("_Color", color);
                transform.Find("Model").Find("orb").GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
                var grad = new Gradient();
                var colorkey = new GradientColorKey[2];
                colorkey[0].color = color;
                colorkey[0].time = 0;
                colorkey[1].color = color;
                colorkey[1].time = 1;
                var alphakey = new GradientAlphaKey[2];
                alphakey[0].alpha = 1;
                alphakey[0].time = 0;
                alphakey[1].alpha = 0;
                alphakey[1].time = 1;
                grad.SetKeys(colorkey, alphakey);
                transform.Find("Model").Find("orb").Find("Particle").GetComponent<Renderer>().material = Damage;
                var col = transform.Find("Model").Find("orb").Find("Particle").GetComponent<ParticleSystem>().colorOverLifetime;
                col.color = grad;
                transform.Find("Model").Find("orb").Find("Particle").GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
                break;
            case Type.Speed:
                color = new Color(0, 1, 191/255);
                propertyBlock = new MaterialPropertyBlock();
                propertyBlock.SetColor("_Color", color);
                transform.Find("Model").Find("orb").GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
                grad = new Gradient();
                colorkey = new GradientColorKey[2];
                colorkey[0].color = color;
                colorkey[0].time = 0;
                colorkey[1].color = color;
                colorkey[1].time = 1;
                alphakey = new GradientAlphaKey[2];
                alphakey[0].alpha = 1;
                alphakey[0].time = 0;
                alphakey[1].alpha = 0;
                alphakey[1].time = 1;
                grad.SetKeys(colorkey, alphakey);
                transform.Find("Model").Find("orb").Find("Particle").GetComponent<Renderer>().material = Speed;
                col = transform.Find("Model").Find("orb").Find("Particle").GetComponent<ParticleSystem>().colorOverLifetime;
                col.color = grad;
                transform.Find("Model").Find("orb").Find("Particle").GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isDrinking = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isDrinking = false;
        GetComponent<AudioSource>().Stop();
        drinkingTime = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if (transform.parent && transform.parent.CompareTag("ControllerHitbox") && other.CompareTag("MainCamera") && isDrinking && canDrink && !isEmpty) {
            canDrink = false;
            GetComponent<AudioSource>().Play();
            StartCoroutine(Drinking());
        }
    }

    IEnumerator Drinking() {
        SteamVR_Input._default.outActions.Haptic.Execute(0, 0.1f, 320, 1, controller);
        drinkingTime++;
        if (drinkingTime >= 2)
        {
            SteamVR_Input._default.outActions.Haptic.Execute(0, 0.1f, 320, 1, controller);
            isEmpty = true;
            Destroy(gameObject.transform.Find("Model").Find("orb").gameObject);
            GetComponent<AudioSource>().Stop();
            if (potionType == Type.Health)
            {
                int toheal = 100;
                if (potionSize == Size.Big)
                    toheal += 100;
                GameObject.Find("Camera").GetComponent<PlayerScript>().health += toheal;
            }
            else if (potionType == Type.Damage)
            {
                int damageBonus = 10;
                if (potionSize == Size.Big)
                    damageBonus += 10;
                StartCoroutine(GameObject.Find("Camera").GetComponent<PlayerScript>().DamagePotion(damageBonus));
            }
            else if (potionType == Type.Speed)
            {
                int speedBonus = 10;
                if (potionSize == Size.Big)
                    speedBonus += 10;
                StartCoroutine(GameObject.Find("Camera").GetComponent<PlayerScript>().StepPotion(speedBonus));
            }
        }
        yield return new WaitForSeconds(1);
        canDrink = true;
    }
}
