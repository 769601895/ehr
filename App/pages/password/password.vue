<template>
	<view class="pd-lr-20">
        <view style="height: 20rpx"></view>
		<view class="mg-t-20" style="display: flex;align-items: baseline;">
            <text class="icon">🔒</text>
            <span style="margin-left: 4rpx;">原密码</span>
		</view>
        <input class="mg-t-20 input" v-model="state.formData.paswOld" placeholder="输入原密码" />
        <view style="height: 20rpx"></view>

		<view class="mg-t-20" style="display: flex;align-items: baseline;">
            <text class="icon">🔒</text>
            <span style="margin-left: 4rpx;">新密码</span>
		</view>
        <input class="mg-t-20 input" v-model="state.paswNew" placeholder="请输入新的密码"
            @blur="newCode" />
        <input class="mg-t-20 input" v-model="state.formData.paswNew" placeholder="再次输入确认密码"
            @blur="newCode2" />

		<view class="txt">
			<view :class="state.isTrue?'':'red'"> {{state.tstxt}} </view>
		</view>
        <view style="height: 40rpx"></view>
        <button class="mg-t-20 btn-primary" @click="onsubmit">提交</button>
	</view>
</template>

<script lang="ts" setup>
	import { reactive } from 'vue';
	import { sm2 } from 'sm-crypto'
    import { env } from '@/utils/.env.js'
    import { useappAuthApi } from '@/api_all/appAuth'

    const state = reactive({
		isTrue: false,
		tstxt: '',
		paswNew: '',
		formData: {} as any,
    })

    const appAuthApi = useappAuthApi()

	const newCode = () => {
		if (state.paswNew.length >= 6 && state.paswNew.length <= 15) {
			state.tstxt = ''
			state.isTrue = true
		} else {
			state.isTrue = false
			state.tstxt = '请输入6-15位，字母或数字'
		}
	}

	const newCode2 = () => {
		if (state.formData.paswNew != state.paswNew) {
			state.isTrue = false
			state.tstxt = '两次密码不一致'
		} else {
			state.tstxt = ''
			state.isTrue = true
		}
	}

	const onsubmit = () => {
		if (!state.isTrue) return uni.showToast({ title: state.tstxt, icon: 'none' })
		let publicKey = env.smKey
		state.formData.passwordOld = sm2.doEncrypt(state.formData.paswOld, publicKey, 1)
		state.formData.passwordNew = sm2.doEncrypt(state.formData.paswNew, publicKey, 1)
        appAuthApi.changePwd(state.formData).then((res : any) => {
            if (res.code == 200) {
                uni.showToast({ title: '修改成功', icon: 'none' })
                setTimeout(() => {
                    uni.navigateBack()
                }, 1000)
            }
        })
    }
</script>

<style lang="scss" scoped>
.icon { font-size: 35rpx; margin-top: 2px; margin-right: 5px; }

	.red {
		color: red;
	}

	.pd-lr-20 {
		padding: 0 16rpx;
	}

	.mg-t-20 {
		margin-top: 20rpx;
	}

.btn-primary { color: #fff; width: 90%; border-radius: 30px; background: $uni-color-primary; height: 90rpx; line-height: 90rpx; }
.input { border: 1px solid #eee; border-radius: 8rpx; padding: 20rpx; }
</style>
