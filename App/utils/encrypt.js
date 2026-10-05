import CryptoJS from 'crypto-js';

var _key = "3141C11DC1A79A7C";
var keyMd5 = CryptoJS.MD5(_key).toString().toUpperCase().substring(0, 16); //DES-8
const key = CryptoJS.enc.Utf8.parse(keyMd5);
const iv = CryptoJS.enc.Utf8.parse(keyMd5.split("").reverse().join(""));

// AES加密
export function AESEncrypt(str) {
	if (str) {
		var encrypt = CryptoJS.AES.encrypt(str, key, {
			iv: iv,
			mode: CryptoJS.mode.CBC,
			padding: CryptoJS.pad.Pkcs7
		});
		return encrypt.toString();
	} else {
		return '';
	}
}

// AES解密
export function AESDecrypt(str) {
	if (str) {
		let decrypted = CryptoJS.AES.decrypt(str, key, {
			iv: iv,
			mode: CryptoJS.mode.CBC,
			padding: CryptoJS.pad.Pkcs7
		});
		return decrypted.toString(CryptoJS.enc.Utf8);
	} else {
		return '';
	}
}