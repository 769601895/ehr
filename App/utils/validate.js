// 身份证号
export const IDcardVal =
	/^\d{6}((((((19|20)\d{2})(0[13-9]|1[012])(0[1-9]|[12]\d|30))|(((19|20)\d{2})(0[13578]|1[02])31)|((19|20)\d{2})02(0[1-9]|1\d|2[0-8])|((((19|20)([13579][26]|[2468][048]|0[48]))|(2000))0229))\d{3})|((((\d{2})(0[13-9]|1[012])(0[1-9]|[12]\d|30))|((\d{2})(0[13578]|1[02])31)|((\d{2})02(0[1-9]|1\d|2[0-8]))|(([13579][26]|[2468][048]|0[048])0229))\d{2}))(\d|X|x)$/;

// 手机号
export const PhoneVal =
	/^(?:(?:\+|00)86)?1(?:(?:3[\d])|(?:4[5-79])|(?:5[0-35-9])|(?:6[5-7])|(?:7[0-8])|(?:8[\d])|(?:9[189]))\d{8}$/;


// 解析身份证
export function judgementIdCard(idCard: string) {
	let entity = {} as any;
	let currentDate = new Date();
	let yearNow = currentDate.getFullYear();
	let birthDateCode = idCard.substring(6, 14);
	let genderCode = parseInt(idCard.substring(16, 17), 10);

	entity.sex = genderCode % 2 === 0 ? 2 : 1;
	entity.age = yearNow - parseInt(birthDateCode.substring(0, 4));
	entity.birthday =
		`${birthDateCode.substring(0, 4)}-${birthDateCode.substring(4, 6)}-${birthDateCode.substring(6, 8)}`;
	return entity;
}