using System;
using UnityEngine;
using System.Collections;
using System.IO;
using QRCodeReaderAndGenerator;
using UnityEngine.UI;

public class QRCodeGeneratorExample : MonoBehaviour {

	[SerializeField]
	InputField inputField;

	[SerializeField]
	RawImage image;

	public void GenerateQRCode()
	{
		if (inputField && image) {
			image.texture = QRCodeManager.Instance.GenerateQRCode (inputField.text);
		} else {
			Debug.Log ("Assign Input Field or Image.");
		}
	}

	public void GenerateQRCodePayloads_WIFI()
	{
		if (image) {
			
			image.texture = QRCodeManager.Instance.GenerateQRCode_WiFi ("Wifi-SSID","myPassword",WIFI_Authentication.WPA);
		} else {
			Debug.Log ("Assign Image.");
		}
	}


	public void GenerateQRCodePayloads_SMS()
	{
		if (image) {

			image.texture = QRCodeManager.Instance.GenerateQRCode_SMS ("123123123","Message",SMS_Encoding.SMS);
		} else {
			Debug.Log ("Assign Image.");
		}
	}

	public void GenerateQRCodePayloads_SkypeCall()
	{
		if (image) {

			image.texture = QRCodeManager.Instance.GenerateQRCode_SkypeCall ("skypename");
		} else {
			Debug.Log ("Assign Image.");
		}
	}

	public void GenerateQRCodePayloads_PhoneCall()
	{
		if (image) {

			image.texture = QRCodeManager.Instance.GenerateQRCode_PhoneNumber ("123123123");
		} else {
			Debug.Log ("Assign Image.");
		}
	}

	public void GenerateQRCodePayloads_MMS()
	{
		if (image) {

			image.texture = QRCodeManager.Instance.GenerateQRCode_MMS ("123123123","Message",MMS_Encoding.MMS);
		} else {
			Debug.Log ("Assign Image.");
		}
	}

	public void GenerateQRCodePayloads_Mail()
	{
		if (image) {

			image.texture = QRCodeManager.Instance.GenerateQRCode_Mail ("abc@gmail.com","Test Message","Message from QRCode",Mail_Encoding.MAILTO);
		} else {
			Debug.Log ("Assign Image.");
		}
	}


	// Next scene
	public void OnPayloadGeneratorClick()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene (1);
	}

	public void OnStringQRGeneratorClick()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene (0);
	}

	public void OnQRCodeScannerClick()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene (2);
	}
}
