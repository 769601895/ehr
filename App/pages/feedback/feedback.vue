<template>
	<view class="pd-lr-20">
		<view class="cardBox mg-t-20">
			<view class="card-title">问题原因</view>
            <view class="line dashed"></view>
			<view class="mg-t-20">
				<view class="card-type">
					<input type="text" v-model="state.fromData.reason" placeholder="描述问题原因" />
				</view>
			</view>
		</view>
		<view class="cardBox mg-t-20">
			<view class="card-title">问题描述</view>
            <view class="line dashed"></view>
			<view class="mg-t-20">
				<textarea v-model="state.fromData.describe" placeholder="描述具体的问题描述"></textarea>
			</view>
		</view>
		<view class="cardBox mg-t-20">
			<view class="card-title">联系方式 </view>
            <view class="line dashed"></view>
			<view class="mg-t-20">
				<input type="number" maxlength="11" v-model="state.fromData.phone" placeholder="请输入手机号码" />
			</view>
		</view>

		<div class="fixed">
            <button class="mg-t-20 btn-primary">提交</button>
		</div>
	</view>
</template>

<script lang="ts" setup>
	import { onMounted, reactive } from 'vue';

	const state = reactive({
		fromData: {} as any,
	})
</script>

<style lang="scss" scoped>
	.success {
		width: 90%;
	}

	.pd-lr-20 {
		padding: 0 16rpx;
	}

	.mg-t-20 {
		margin-top: 20rpx;
	}

	.cardBox {
		padding: 20rpx;
		background: #fff;
		border-radius: 15rpx;
		box-sizing: border-box;
	}

	.card-title {
		color: $uni-color-primary;
		position: relative;
		padding-left: 20rpx;
		padding: 10rpx 0 30rpx 20rpx;
		font-weight: bold;
		font-size: 32rpx;
		height: 32rpx;
		line-height: 32rpx;
	}

	.card-title::before {
		content: '';
		position: absolute;
		left: 0;
		width: 6rpx;
		height: 32rpx;
		border-radius: 0 6rpx 6rpx 0;
		background: $uni-color-primary;
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

.btn-primary { color: #fff; width: 90%; border-radius: 30px; background: $uni-color-primary; height: 90rpx; line-height: 90rpx; }
.line { border-top: 1px dashed #ccc; }
</style>
