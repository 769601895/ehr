<template>
	<view style="height:100vh; background:#fff;">
		<view class="img-a">
            <image class="img-a-bg" src="/static/bg.jpg" mode="scaleToFill"></image>
			<view class="t-b">
				欢迎使用 <br />
				<div style="font-size: 55rpx">Admin.NET 开发框架</div>
			</view>
		</view>
		<view class="login-view">
			<view class="t-login">
				<form class="cl">
					<view class="t-a">
                        <view class="txt">
                            <text class="icon">👤</text> <span style="margin-left: 4rpx;">账号</span>
                        </view>
						<input name="account" placeholder="请输入账号" v-model="state.formData.account" />
					</view>
					<view class="t-a">
                        <view class="txt">
                            <text class="icon">🔒</text> <span style="margin-left: 4rpx;">密码</span>
                        </view>
						<input type="password" name="password" maxlength="18" placeholder="请输入密码" v-model="state.password" />
					</view>
					<view style="display: flex;">
						<input type="number" name="code" maxlength="2" placeholder="请输入验证码" v-model="state.formData.code" />
						<div class="captImg" :style="'background-image: url('+state.captchaImage+');'" @click="initCaptcha">
							<div v-if="state.expirySeconds <= 0" class="captImg-expired-overlay">验证码过期</div>
						</div>
					</view>

                    <button class="btn-primary" @tap="login">登 录</button>
				</form>

				<view class="t-e cl" v-if="true">
					<view class="divider">第三方账号登录</view>

					<view class="t-g" @tap="wxLogin">
						<svg class="third-icon wx-icon" viewBox="0 0 24 24">
							<path d="M4 4h16a2 2 0 012 2v8a2 2 0 01-2 2H9l-5 5V6a2 2 0 012-2z" />
						</svg>
					</view>
					<view class="t-g" @tap="zfbLogin">
						<svg class="third-icon qq-icon" viewBox="0 0 24 24">
							<path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z" />
						</svg>
					</view>
				</view>

				<view class="mg-t-20 fixed">
					<view class="flex-center" @click="callPhone">
						联系电话：{{ state.servicePhone }}
					</view>
					<view class="flex-center copyright mg-t-10"> Copyright © 2024 Admin.NET All rights reserved. </view>
				</view>
			</view>
		</view>
	</view>
</template>

<script lang="ts" setup>
	import { reactive, onMounted, onUnmounted } from 'vue';
	import { env } from '@/utils/.env.js'
	import { sm2 } from 'sm-crypto'
    import { accessTokenKey, refreshAccessTokenKey, currentUserKey, clearAccessTokens } from '@/utils/request.js'
    import { usesysAuthApi } from '@/api_all/sysAuth'

    const state = reactive({
        password: env.defaultPassword,
		captchaImage: '', // 验证码
        formData: {
            account: env.defaultUser,
            password: '',
            codeId: 0,
            code: 'string'
        },
        servicePhone: env.defaultServicePhone, // 服务电话
        expirySeconds: 0, // 验证码过期时间
    })

    const authApi = usesysAuthApi()
    let timer : any = null;

    // 页面初始化
	onMounted(() => {
		initCaptcha()
	});

    onUnmounted(() => {
        if(timer) clearInterval(timer);
    });

	// 获取验证码
    const initCaptcha = () => {
        if(timer) clearInterval(timer);
        authApi.captcha().then((res : any) => {
            state.captchaImage = 'data:text/html;base64,' + res.result?.img;
            state.formData.codeId = res.result?.id;
            state.expirySeconds = res.result?.expirySeconds ?? 60;
            
            timer = setInterval(() => {
                if (state.expirySeconds > 0) {
                    state.expirySeconds--;
                } else {
                    clearInterval(timer);
                }
            }, 1000);
        })
    }

	// 登录系统
	const login = async () => {
		// 添加主题色
		uni.setStorageSync('theme', '#11559c')

		// 密码加密传输
		var password = sm2.doEncrypt(state.password, env.smKey, 1);
		state.formData.password = password;

        try {
            const res : any = await authApi.login(state.formData)
            uni.setStorageSync(accessTokenKey, res.result.accessToken)
            uni.setStorageSync(refreshAccessTokenKey, res.result.refreshToken)

            try {
                const u : any = await authApi.userInfo(undefined, false, false)
                if (!u || u.code !== 200 || !u.result) {
                    throw new Error('获取用户信息失败')
                }
                uni.setStorageSync(currentUserKey, u.result)
                uni.switchTab({ url: '/pages/home/home' })
            } catch (err) {
                clearAccessTokens()
                uni.showToast({ title: '获取用户信息失败，登录失败', icon: 'none' })
            }
        } finally {
            initCaptcha()
        }
    }

	// 拨打电话
	const callPhone = () => {
		uni.makePhoneCall({
			phoneNumber: state.servicePhone
		});
	}

	// 微信登录
	const wxLogin = () => {
		uni.showToast({ title: '点击了微信！', icon: 'none' });
	}

	// 支付宝登录
	const zfbLogin = () => {
		uni.showToast({ title: '点击了支付宝！', icon: 'none' });
	}
</script>

<style lang="scss" scoped>
	.captImg {
		width: 300rpx;
		height: 80rpx;
		background-size: cover;
        cursor: pointer;
        position: relative;
	}

    .captImg-expired-overlay {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background-color: rgba(0, 0, 0, 0.65);
        color: #fff;
        font-size: 28rpx;
        display: flex;
        align-items: center;
        justify-content: center;
        text-align: center;
        font-weight: bold;
        letter-spacing: 4rpx;
    }

.txt {
    display: flex;
    align-items: end;
    font-size: 32rpx;
    color: #666;
}

.icon {
    font-size: 40rpx;
}

.btn-primary {
    font-size: 28rpx;
    background: #2796f2;
    color: #fff;
    height: 90rpx;
    line-height: 90rpx;
    border-radius: 50rpx;
    font-weight: bold;
    width: 100%;
    cursor: pointer;
}

.divider {
    margin-top: 150rpx;
    text-align: center;
    color: #999;
}

	.flex-center {
		color: #999;
		font-size: 14rpx;
		text-decoration: none;
		// align-items: baseline;
		text-align: center;
		align-items: center;
	}

	.copyright {
		color: #999;
		font-size: 14rpx;
	}

	.img-a {
		width: 100%;
		height: 500rpx;
        position: relative;
	}

    .img-a-bg {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        z-index: 0;
    }

	.reg {
		font-size: 28rpx;
		color: #fff;
		height: 90rpx;
		line-height: 90rpx;
		border-radius: 50rpx;
		font-weight: bold;
		background: #f5f6fa;
		color: #000000;
		text-align: center;
		margin-top: 30rpx;
	}

	.login-view {
		width: 100%;
		position: relative;
		margin-top: -80rpx;
		background-color: #ffffff;
		border-radius: 8% 8% 0% 0;
	}

	.t-login {
		width: 600rpx;
		margin: 0 auto;
		font-size: 28rpx;
		padding-top: 80rpx;
	}

	.t-login button {
		font-size: 28rpx;
		background: #2796f2;
		color: #fff;
		height: 90rpx;
		line-height: 90rpx;
		border-radius: 50rpx;
		font-weight: bold;
	}

	.t-login input {
		height: 90rpx;
		line-height: 90rpx;
		margin-bottom: 50rpx;
		border-bottom: 1px solid #e9e9e9;
		font-size: 28rpx;
        cursor: text;
	}

	.t-login .t-a {
		position: relative;
	}

	.t-b {
		text-align: left;
		font-size: 42rpx;
		color: #ffffff;
		padding: 200rpx 0 0 70rpx;
		font-weight: bold;
		line-height: 70rpx;
        position: relative;
        z-index: 1;
	}

	.t-login .t-c {
		position: absolute;
		right: 22rpx;
		top: 22rpx;
		background: #5677fc;
		color: #fff;
		font-size: 24rpx;
		border-radius: 50rpx;
		height: 50rpx;
		line-height: 50rpx;
		padding: 0 25rpx;
	}

	.t-login .t-d {
		text-align: center;
		color: #999;
		margin: 80rpx 0;
	}

	.t-login .t-e {
		text-align: center;
		margin: 80rpx auto 0;
	}

	.t-login .t-g {
		float: left;
		width: 50%;
        cursor: pointer;
	}

	.t-login .t-e image {
		width: 50rpx;
		height: 50rpx;
	}

	.t-login .t-e .third-icon {
		width: 50rpx;
		height: 50rpx;
		fill: currentColor;
	}

	.wx-icon { color: $uni-color-success; }
	.qq-icon { color: $uni-color-primary; }

	.t-login .uni-input-placeholder {
		color: #aeaeae;
	}

	.cl {
		zoom: 1;
	}

	.cl:after {
		clear: both;
		display: block;
		visibility: hidden;
		height: 0;
		content: '\20';
	}

	.fixed {
		width: 100vw;
		padding: 10rpx 20rpx 30rpx;
		background: #fff;
		box-sizing: border-box;
		position: fixed;
		bottom: 0;
		left: 0;
	}

    /* removed uview-plus button style */
</style>
