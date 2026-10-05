<template>
  <view class="card">
    <view class="card-title">经营概况
      <text class="refresh" @click="refresh">刷新</text>
    </view>
    <view class="card-body">
      <view class="metric metric-row">
        <text class="metric-label">相关设备总量</text>
        <text class="metric-value">{{ state.data.deviceCount }}</text>
      </view>
      <view class="metric metric-row">
        <text class="metric-label">在线设备数量</text>
        <text class="metric-value">{{ state.data.onlineDeviceCount }}</text>
      </view>
      <view class="metric metric-row">
        <text class="metric-label">订单总数</text>
        <text class="metric-value">{{ state.data.orderCount }}</text>
      </view>
      <view class="metric metric-row">
        <text class="metric-label">未结算金额</text>
        <text class="metric-value amount">{{ formatAmount(state.data.orderNotPayValue) }} 元</text>
      </view>
    </view>
  </view>
</template>

<script lang="ts" setup>
import { reactive, onMounted } from 'vue'
import { usepyStatisticsApi } from '@/api_all/appH5Demo'

type BusinessOverviewData = {
  deviceCount: number
  onlineDeviceCount: number
  orderCount: number
  orderTotalValue: number
  orderNotPayValue: number
}

const state = reactive<{ data: BusinessOverviewData, loading: boolean }>({
  data: {
    deviceCount: 0,
    onlineDeviceCount: 0,
    orderCount: 0,
    orderTotalValue: 0,
    orderNotPayValue: 0
  }
})

const statsApi = usepyStatisticsApi()

const refresh = async () => {
  uni.showLoading({ title: '加载中' })
  
  try {
    const res: any = await statsApi.businessStatus({}, false, false)
    if (res && res.result) {
      state.data = res.result as BusinessOverviewData
    }
  }
  catch(e) {
    uni.showToast({ title: '获取经营概况失败：' + e.errMsg, icon: 'none' })
  } finally {
    uni.hideLoading()
  }
}

onMounted(() => {
  //refresh()
})

const formatAmount = (v: number) => {
  if (!isFinite(v)) return '0.00'
  return Number(v).toFixed(2)
}
</script>

<style lang="scss" scoped>
.card {
  background: #fff;
  border-radius: 15rpx;
  box-shadow: 0 6rpx 20rpx rgba(0,0,0,0.06);
  padding: 20rpx;
}

.card-title {
  position: relative;
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

.card-body {
  display: flex;
  flex-wrap: wrap;
}

.metric {
  width: 50%;
  padding: 20rpx;
  box-sizing: border-box;
  display: flex;
  align-items: baseline;
  justify-content: space-between;
}

.metric-row {
  width: 100%;
  display: flex;
  align-items: baseline;
  justify-content: space-between;
}

.metric-row .metric-value {
  margin-top: 0;
  margin-left: 20rpx;
}

.metric .metric-value {
  margin-top: 0;
  margin-left: 20rpx;
}

.metric-label {
  color: #999;
  font-size: 26rpx;
}

.metric-value {
  margin-top: 10rpx;
  font-size: 40rpx;
  font-weight: bold;
  color: #333;
}

.amount {
  color: $uni-color-primary;
}

.refresh {
  position: absolute;
  right: 20rpx;
  top: 10rpx;
  height: 32rpx;
  line-height: 32rpx;
  color: $uni-color-primary;
  font-size: 26rpx;
  cursor: pointer;
}

.refresh.disabled {
  opacity: 0.6;
  pointer-events: none;
}
</style>
