export function GetUrlParam(name) {
	// var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
	// var r = window.location.search.substr(1).match(reg);
	// if (r != null) return unescape(r[2]);
	// return null;
	var reg = new RegExp("(^|\\?|&)" + name + "=([^&]*)(\\s|&|$)", "i");
	if (reg.test(decodeURI(window.location)))
		return unescape(RegExp.$2.replace(/\+/g, " ")).split('#')[0]; //去掉#后面的
	return "";
}


export function getRequestParams(url) {
	// 解析URL地址，获取参数部分
	const paramsString = url.split('?')[1];

	// 解析参数字符串，获取参数对象
	const params = {};
	paramsString.split('&').forEach(param => {
		const [key, value] = param.split('=');
		params[key] = decodeURIComponent(value);
	});
	return params;
}