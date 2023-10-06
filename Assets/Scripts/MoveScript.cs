using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MoveScript : MonoBehaviour {

    public SteamVR_Input_Sources controller;

    public float maxStepDistance = 2f;
    public float maxHeightDistance = 1f;
    private float maxStepDistanceDefault = 2f;
    private float maxHeightDistanceDefault = 1f;
    public GameObject player;
    public Transform CastFromObject;
	public GameObject otherController;
	public GameObject hexagonHighlight;
	public GameObject currentHexagon;
	private GameObject selectedHexagon;
	private GameObject selectedObject;
	private LineRenderer line;
	private bool teleporting;

    private void Start()
    {
		selectedHexagon = null;
        line = GetComponent<LineRenderer>();
		line.material.color = Color.green;
		teleporting = false;
    }

	private void SetHightlight(GameObject obj){
		if (obj && obj.CompareTag ("Floor")) {
            if (obj != selectedHexagon)
            {
                player.transform.Find("Camera").GetComponent<AudioSource>().clip = player.transform.Find("Camera").GetComponent<PlayerScript>().switchEffect;
                player.transform.Find("Camera").GetComponent<AudioSource>().Play();
                SteamVR_Input._default.outActions.Haptic.Execute(0, Time.deltaTime, 320, 1, controller);
                selectedHexagon = obj;
                hexagonHighlight.transform.position = selectedHexagon.transform.position + new Vector3(0, selectedHexagon.GetComponent<MeshRenderer>().bounds.extents.y * 2, 0);
                hexagonHighlight.SetActive(true);
            }
		} else {
			selectedHexagon = null;
			hexagonHighlight.SetActive(false);
		}
	}

	private void DrawRoundedLine(Vector3 middlePos, Vector3 endPos){
		List<Vector3> pointList = new List<Vector3>();
		for (float ratio = 0; ratio < 1; ratio += 1.0f / 25)
		{
			Vector3 tangentLineVertex1 = Vector3.Lerp(CastFromObject.position, middlePos, ratio);
			Vector3 tangentLineVertex2 = Vector3.Lerp(middlePos, endPos, ratio);
			Vector3 bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
			pointList.Add(bezierPoint);
		}
        pointList.Add(endPos);
		line.positionCount = pointList.Count;
		line.SetPositions(pointList.ToArray());
	}

	private Vector3 CreateRayDown(Vector3 originPos){
		RaycastHit hit;
		Ray ray = new Ray(originPos, Vector3.down);
        if (Physics.Raycast(ray, out hit))
        {
            originPos = hit.point;
            //GameObject.Find("PositionBall").transform.position = originPos;
            float objectHeightDistance = Vector3.Distance(new Vector3(0, currentHexagon.transform.position.y + currentHexagon.GetComponent<MeshRenderer>().bounds.extents.y * 2, 0), new Vector3(0, originPos.y, 0));
            if (hit.transform.CompareTag("Floor") && objectHeightDistance <= maxHeightDistance)
            {
                selectedObject = hit.transform.gameObject;
                hexagonHighlight.SetActive(true);
                line.material.color = Color.green;
            }
            else
            {
                selectedObject = null;
                hexagonHighlight.SetActive(false);
                line.material.color = Color.red;
            }
        }
        else {
            selectedObject = null;
            hexagonHighlight.SetActive(false);
            line.material.color = Color.red;
        }
        return originPos;
	}

	private void DrawLine(){
        float height = Vector3.Distance(new Vector3(0, CastFromObject.position.y, 0), new Vector3(0, (currentHexagon.transform.position.y + currentHexagon.GetComponent<MeshRenderer>().bounds.extents.y * 2), 0));
        RaycastHit hit;
		Ray ray = new Ray (CastFromObject.position, transform.forward);
		if (Physics.Raycast (ray, out hit)) {
            if (hit.transform.parent && hit.transform.parent.name == "MapParts")
            {
                selectedObject = hit.transform.gameObject;
                float distance = Vector3.Distance(new Vector3(CastFromObject.position.x, 0, CastFromObject.position.z), new Vector3(hit.point.x, 0, hit.point.z));
                float objectHeightDistance = Vector3.Distance(new Vector3(0, currentHexagon.transform.position.y + currentHexagon.GetComponent<MeshRenderer>().bounds.extents.y * 2, 0), new Vector3(0, selectedObject.transform.position.y + selectedObject.GetComponent<MeshRenderer>().bounds.extents.y * 2, 0));
                //if (ray.GetPoint(Mathf.Sqrt((distance / 2) * (distance / 2) + height * height)).y <= (hit.point.y + transform.position.y) / 2 && distance <= maxStepDistance)
                if (distance <= maxStepDistance)
				{
					if (selectedObject.CompareTag ("Floor") && objectHeightDistance <= maxHeightDistance) {
						line.material.color = Color.green;
					} else {
                        selectedObject = null;
						line.material.color = Color.red;
					}
                    //GameObject.Find("PositionBall").transform.position = hit.point;

                    SetHightlight(selectedObject);
					line.positionCount = 2;
					line.SetPosition(0, CastFromObject.position);
					line.SetPosition(1, hit.point);
                }
                else
                {
                    distance = Mathf.Clamp(distance, 0, maxStepDistance);
                    Vector3 endPos = ray.GetPoint(Mathf.Sqrt(distance * distance + height * height));
                    Vector3 middlePos = ray.GetPoint(Mathf.Sqrt((distance / 2) * (distance / 2) + height * height));

					endPos = CreateRayDown (endPos);
                    SetHightlight(selectedObject);
					DrawRoundedLine (middlePos, endPos);
                }
            }
		} else {
			selectedObject = null;
            Vector3 endPos = ray.GetPoint(Mathf.Sqrt(maxStepDistance * maxStepDistance + height * height));
            Vector3 middlePos = ray.GetPoint(Mathf.Sqrt((maxStepDistance / 2) * (maxStepDistance / 2) + height * height));

			endPos = CreateRayDown (endPos);
            SetHightlight(selectedObject);
			DrawRoundedLine (middlePos, endPos);
        }
	}

	void FixedUpdate () {
        maxStepDistance = maxStepDistanceDefault * (GameObject.Find("Camera").GetComponent<PlayerScript>().stepBonus / 10 + 1);
        maxHeightDistance = maxHeightDistanceDefault * (GameObject.Find("Camera").GetComponent<PlayerScript>().stepBonus / 20 + 1);
		if (SteamVR_Input._default.inActions.Teleport.GetState (controller) && !otherController.GetComponent<MoveScript>().IsTeleporting()) {
			teleporting = true;
            transform.GetChild(0).GetComponent<Animator>().SetBool("Point", true);
			DrawLine ();
		}
		if (SteamVR_Input._default.inActions.Teleport.GetStateUp (controller) && teleporting)
		{
            if (selectedHexagon)
            {
                player.transform.Find("Camera").GetComponent<AudioSource>().clip = player.transform.Find("Camera").GetComponent<PlayerScript>().teleport;
                player.transform.Find("Camera").GetComponent<AudioSource>().Play();
                currentHexagon = selectedHexagon;
                otherController.GetComponent<MoveScript>().currentHexagon = selectedHexagon;
                player.transform.position = new Vector3(selectedHexagon.transform.position.x + (player.transform.position.x - player.transform.Find("Camera").position.x), (currentHexagon.transform.position.y + currentHexagon.GetComponent<MeshRenderer>().bounds.extents.y * 2), selectedHexagon.transform.position.z + (player.transform.position.z - player.transform.Find("Camera").position.z));
            }

			hexagonHighlight.SetActive(false);
			line.positionCount = 0;
			teleporting = false;
            transform.GetChild(0).GetComponent<Animator>().SetBool("Point", false);
        }
    }

	public bool IsTeleporting(){
		return teleporting;
	}
}
