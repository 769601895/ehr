<template>
	<view class="content">
		<image class="logo" src="/static/logo.png"></image>
		<view class="text-area">
			<text class="title" style="font-weight: bold;">{{ state.title }}</text>
		</view>
		<text style="font-size: 12px;color: #FFFFFF;">Copyright © 2024 Admin.NET All rights reserved.</text>
		<br />
		<view class="text-area">
			<text style="font-size: 12px;color: #FFFFFF; position: absolute; margin: 0; bottom: 20px;">始创于2022年10月10日</text>
		</view>
	</view>
</template>

<script lang="ts" setup>
	import { reactive } from 'vue';

	const state = reactive({
		title: 'Admin.NET'
	})
</script>

<style lang="scss" scoped>
	.content {
		display: flex;
		flex-direction: column;
		align-items: center;
		height: 100vh;
		background-color: #23212F;
	}

	.logo {
		height: 200rpx;
		width: 200rpx;
		margin-top: 200rpx;
		margin-left: auto;
		margin-right: auto;
		margin-bottom: 50rpx;
	}

	.text-area {
		display: flex;
		justify-content: center;
	}

	.title {
		font-size: 37rpx;
		color: #FFFFFF;
	}
</style>