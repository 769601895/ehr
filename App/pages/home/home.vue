<template>
	<view>
    <swiper indicator-dots circular>
        <swiper-item v-for="(src, i) in state.swiperList" :key="i">
            <image :src="src" style="width:100%;height:200rpx" mode="aspectFill" />
        </swiper-item>
    </swiper>

    <view class="notice">{{ state.notice }}</view>

    <view class="grid">
        <view class="grid-item" v-for="(item,index) in state.gridList" :key="index">
            <view class="grid-icon" @click="onGridItemClick(item)">{{ item.title.slice(0,1) }}</view>
            <text class="grid-text">{{ item.title }}</text>
        </view>
    </view>
		<BusinessOverview />
		<OrderOverview />
		<SysLogOp />
	</view>


	<TabBar :current-page="0" />
</template>

<script lang="ts" setup>
	import { onMounted, reactive } from 'vue';
	import TabBar from '@/pages/component/tabbar.vue'
	import SysLogOp from '@/pages/component/sysLogOp.vue'
	import BusinessOverview from '@/pages/component/BusinessOverview.vue'
	import OrderOverview from '@/pages/component/OrderOverview.vue'
	const state = reactive({
		swiperList: [
			'/static/swiper/swiper1.png',
			'/static/uview/swiper/swiper2.png',
			'/static/uview/swiper/swiper3.png',
		],
		gridList: [
			{
				name: 'red-packet-fill',
				title: '功能1',
				url: '/'
			},
			{
				name: 'zhifubao-circle-fill',
				title: '功能2',
				url: '/'
			},
			{
				name: 'apple-fill',
				title: '功能3',
				url: '/'
			},
			{
				name: 'twitter-circle-fill',
				title: '功能4',
				url: '/'
			},
			{
				name: 'taobao-circle-fill',
				title: '功能5',
				url: '/'
			},
			{
				name: 'baidu-circle-fill',
				title: '测试6',
				url: '/'
			},
			{
				name: 'facebook-circle-fill',
				title: '功能7',
				url: '/'
			},
			{
				name: 'weibo-circle-fill',
				title: '功能8',
				url: '/'
			},
		],
		notice: '欢迎使用，Admin.NET 站在巨人肩膀上的 .NET 通用权限开发框架。让开发更简单、更通用、更流行！'
	})

onMounted(() => {
})

const onGridItemClick = (item:any) => {
    if (typeof item.onClick === 'function') {
        item.onClick(item)
    } else {
        uni.showToast({ title: '点击：' + (item.title || ''), icon: 'none' })
    }
}
</script>

<style lang="scss" scoped>
	.grid {
		padding-top: 20rpx;
		background: #fff;
		box-sizing: border-box;
	}

.grid-icon { color: white; width: 80rpx; height: 80rpx; border-radius: 50%; box-sizing: border-box; background: $uni-color-primary; display:flex; align-items:center; justify-content:center; }
.grid { padding-top: 20rpx; background: #fff; box-sizing: border-box; display:flex; flex-wrap:wrap; }
.grid-item { width: 25%; display:flex; flex-direction:column; align-items:center; }
.notice { background:#fff; padding: 20rpx; color:#333; }

	.grid-text {
		font-size: 14px;
		padding: 10rpx 0 40rpx 0rpx;
	}
</style>
