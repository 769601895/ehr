<template>
	<view class="tabbar-container">
		<view class="tabbar-item" v-for="item in state.tabbarList" :class="[item.centerItem ? ' center-item' : '']" @click="changeItem(item)">
        <view class="item-top">
            <svg class="tab-icon" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg" :style="{ color: state.currentItem==item.id ? state.theme : '#999' }">
                <path :d="getIconPath(item.icon)" />
            </svg>
        </view>
			<view class="item-bottom" :class="[state.currentItem==item.id ? 'item-active' : '']">
				<text style="font-size: 30rpx;">{{ item.text }}</text>
			</view>
		</view>
	</view>
</template>

<script lang="ts" setup>
    import { onMounted, reactive } from 'vue';

	const props = defineProps({
		currentPage: Number,
	});

	const state = reactive({
		theme: uni.getStorageSync('theme'),
		currentItem: 0,
		tabbarList: [{
			id: 0,
			path: "/pages/home/home",
			icon: 'home',
			text: "首页",
			centerItem: false
		}, {
			id: 1,
			path: "/pages/mine/mine",
			icon: 'account',
			text: "我的",
			centerItem: false
		}]
	})

    onMounted(() => {
        state.currentItem = props.currentPage;
        uni.hideTabBar();
    });

    const changeItem = (item : any) => {
        uni.switchTab({ url: item.path });
    }

    const getIconPath = (name: string) => {
        switch (name) {
            case 'home':
                return 'M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8h5z'
            case 'account':
            case 'user':
                return 'M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z'
            default:
                return 'M12 2a10 10 0 1 0 0 20 10 10 0 0 0 0-20z'
        }
    }
</script>

<style lang="scss" scoped>
	.tabbar-container {
		position: fixed;
		bottom: 0;
		left: 0;
		width: 100%;
		height: 110rpx;
		box-shadow: 0px 3px 20px rgba(0, 0, 0, 0.16);
		border-top: 1px;
		display: flex;
		align-items: center;
		padding: 5rpx 0;
		color: #999999;
		z-index: 200;
		background-color: #fff;
	}

	.tabbar-container .tabbar-item {
		width: 50%;
		height: 100rpx;
		display: flex;
		flex-direction: column;
		justify-content: center;
		align-items: center;
		text-align: center;
		background-color: #fff;
	}

.item-active {
    color: $uni-color-primary;
}

.tab-icon { width: 40rpx; height: 40rpx; fill: currentColor; }
</style>
