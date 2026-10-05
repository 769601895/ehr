<template>
	<view>
		<view class="mineInfo">
			<view class="userInfo flex" style="justify-content: space-between;">
				<view class="flex-column">
                    <image v-if="state.userInfo.avatar" class="avatar " :src="env.baseUrl + state.userInfo.avatar" />
                    <view v-else class="avatar "> {{ state.userInfo.realName?.slice(0, 1) }} </view>
				</view>

				<view class="flex flex-column" style="width: calc(100% - 150rpx);">
                    <view class="userInfo-name"> {{ state.userInfo.account }} </view>
					<view style="display: flex; align-items: flex-end;">
                        {{ state.userInfo.realName }}
                        <view class="tag mg-l-20">标签</view>
					</view>
				</view>
			</view>
		</view>

		<view class="pd-lr-20" style="margin-top: -80rpx;">
            <view class="mg-t-20">
                <view class="grid pd-t-20">
                    <view class="grid-item" v-for="(item,index) in state.gridList" :key="index">
                        <view class="grid-icon" @click="onGridItemClick(item)">{{ item.iconText || item.title.slice(0,1) }}</view>
                        <text class="grid-text">{{ item.title }}</text>
                    </view>
                </view>
            </view>
		</view>

		<view style="background-color: #FFF; margin-top: 30rpx;">
            <view class="cell-group">
                <view class="cell" @click="uni.navigateTo({url:'/pages/password/password'})">
                    <text>修改密码</text>
                    <text class="cell-arrow">›</text>
                </view>
                <view class="cell" @click="uni.navigateTo({url:'/pages/about/about'})">
                    <text>关于我们</text>
                    <text class="cell-arrow">›</text>
                </view>
                <view class="cell" @click="uni.navigateTo({url:'/pages/feedback/feedback'})">
                    <text>意见反馈</text>
                    <text class="cell-arrow">›</text>
                </view>
                <view class="cell">
                    <text>当前版本</text>
                    <text class="cell-value">v1.0.0</text>
                </view>
                <view class="cell" @click="logout">
                    <text>退出登录</text>
                    <text class="cell-arrow">›</text>
                </view>
            </view>
		</view>
	</view>

	<TabBar :current-page="1" />

</template>

<script lang="ts" setup>
	import { onMounted, reactive } from 'vue';
	import { env } from '@/utils/.env';
    import TabBar from '@/pages/component/tabbar.vue'
    import { usesysAuthApi } from '@/api_all/sysAuth'
    import { currentUserKey } from '@/utils/request.js';

	const state = reactive({
		theme: uni.getStorageSync('theme'),
		userInfo: {} as any,
		gridList: [
            {
                name: 'red-packet-fill',
                title: '1功能',
                onClick: (it:any) => uni.showToast({ title: '点击：' + it.title, icon: 'none' })
            },
            {
                name: 'zhifubao-circle-fill',
                title: '2功能',
                onClick: (it:any) => uni.showToast({ title: '点击：' + it.title, icon: 'none' })
            },
            {
                name: 'apple-fill',
                title: '3功能',
                onClick: (it:any) => uni.showToast({ title: '点击：' + it.title, icon: 'none' })
            },
            {
                name: 'twitter-circle-fill',
                title: '4功能',
                onClick: (it:any) => uni.showToast({ title: '点击：' + it.title, icon: 'none' })
            }
		],
    })

    const authApi = usesysAuthApi()

    const onGridItemClick = (item:any) => {
        if (typeof item.onClick === 'function') {
            item.onClick(item)
        } else {
            uni.showToast({ title: '点击：' + (item.title || ''), icon: 'none' })
        }
    }

	// 页面初始化
    onMounted(() => {
        state.userInfo = uni.getStorageSync(currentUserKey)
        console.log(state.userInfo)

        if (!state.userInfo || (typeof state.userInfo === 'string' && state.userInfo === '')) {
            state.userInfo = {
                'account': 'Admin.NET',
                'phone': '18012345678'
            }
        }
    });

	// 退出系统
	const logout = () => {
		uni.showModal({
			title: '提示',
			content: '是否退出系统？',
			success: (e) => {
				if (!e.confirm) return;

                authApi.logout().then((res : any) => {
                    if (res.code == 200) {
                        uni.showToast({ title: '退出系统', icon: 'none' })
                        uni.clearStorageSync();
                        setTimeout(() => {
                            uni.reLaunch({ url: '/pages/login/login' })
                        }, 1000)
                    }
                })
			}
		})
	}
</script>

<style lang="scss" scoped>
	.flex {
		display: flex;
		flex-wrap: wrap;
	}

	.flex-column {
		display: flex;
		flex-direction: column;
		justify-content: space-around;
	}

	.mg-l-20 {
		margin-left: 20rpx;
	}

	.pd-lr-20 {
		padding: 0 20rpx;
	}

.mineInfo {
		width: 100vw;
		padding: 20rpx 0 100rpx;
		background: $uni-color-primary;

		.userInfo {
			width: 90%;
			height: 120rpx;
			padding: 40rpx 0;
			margin: auto;
			color: white;

			.avatar {
				color: #1661AB;
				background: #FFF;
				text-align: center;
				border-radius: 50%;
				flex: none;
				width: 120rpx;
				height: 120rpx;
				font-size: 50rpx;
				line-height: 120rpx;

                image {
                    width: 100%;
                    height: 100%;
                }
            }

			&-name {
				font-size: 40rpx;
				font-weight: bold;
				// letter-spacing: 1px;
			}
		}
	}

    .grid {
        padding-top: 30rpx;
        background: #fff;
        border-radius: 15rpx;
        box-sizing: border-box;
        display: flex;
        flex-wrap: wrap;
    }

    .grid-item { width: 25%; display: flex; flex-direction: column; align-items: center; }
    .grid-icon { color: #fff; width: 80rpx; height: 80rpx; border-radius: 50%; background: $uni-color-primary; display:flex; align-items:center; justify-content:center; font-size: 30rpx; }

    .grid-text { font-size: 14px; padding: 10rpx 0 40rpx 0rpx; }

    .cell-group { padding: 10rpx 0; background: #fff; border-radius: 15rpx; }
    .cell { padding: 30rpx 20rpx; border-bottom: 1px solid #f0f0f0; display:flex; align-items:center; justify-content:space-between; font-size: 28rpx; }
    .cell-arrow { color: #c0c0c0; font-size: 32rpx; }
    .cell-value { color: #666; }
    .tag { display:inline-block; border: 1px solid #fff; color:#fff; border-radius: 8rpx; padding: 4rpx 10rpx; font-size: 24rpx; }


    
</style>
