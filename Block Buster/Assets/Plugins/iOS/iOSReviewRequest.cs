using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if (UNITY_IOS && !UNITY_EDITOR)
using System.Runtime.InteropServices;
#endif

public class iOSReviewRequest : MonoBehaviour 
{
	
	#if (UNITY_IOS && !UNITY_EDITOR)
    [DllImport ("__Internal")]
    private static extern void requestReview();
	#endif
	
	public static void Request()
	{
		#if (UNITY_IOS && !UNITY_EDITOR)
		Debug.Log("Trying to request the review window.");
	    requestReview();
		#endif
	}
}
