<template>
  <view class="card">
    <view class="card-title">订单概况</view>

    <view class="range">
      <view :class="['range-item', state.rangeType==='all' ? 'active' : '']" @click="setPreset('all')">全部</view>
      <view :class="['range-item', state.rangeType==='1d' ? 'active' : '']" @click="setPreset('1d')">1天</view>
      <view :class="['range-item', state.rangeType==='3d' ? 'active' : '']" @click="setPreset('3d')">3天</view>
      <view :class="['range-item', state.rangeType==='7d' ? 'active' : '']" @click="setPreset('7d')">7天</view>
      <view :class="['range-item', state.rangeType==='custom' ? 'active' : '']" @click="setPreset('custom')">自定义</view>
    </view>

    <view class="date-bar">
      <template v-if="state.rangeType==='custom'">
        <picker mode="date" :value="state.startDate" @change="onStartChange">
          <view class="date-input">{{ state.startDate || '开始日期' }}</view>
        </picker>
        <text class="date-sep">至</text>
        <picker mode="date" :value="state.endDate" @change="onEndChange">
          <view class="date-input">{{ state.endDate || '结束日期' }}</view>
        </picker>
      </template>
      <template v-else-if="state.rangeType==='all'">
        <view class="date-input disabled">全部</view>
      </template>
      <template v-else>
        <view class="date-input disabled">{{ state.startDate }}</view>
        <text class="date-sep">至</text>
        <view class="date-input disabled">{{ state.endDate }}</view>
      </template>

      <button class="query-btn" @click="query">查询</button>
    </view>

    <view class="card-body">
      <view class="metric metric-row">
        <text class="metric-label">订单总数</text>
        <text class="metric-value">{{ state.data.orderCount }}</text>
      </view>
      <view class="metric metric-row">
        <text class="metric-label">订单收入</text>
        <text class="metric-value amount">{{ formatAmount(state.data.orderTotalValue) }} 元</text>
      </view>
    </view>
  </view>
</template>

<script lang="ts" setup>
import { reactive, onMounted } from 'vue'
import dayjs from 'dayjs'
import { usepyStatisticsApi } from '@/api_all/appH5Demo'

type OrderOverviewData = {
  orderCount: number
  orderTotalValue: number
}

const state = reactive<{ data: OrderOverviewData, loading: boolean, rangeType: 'all'|'1d'|'3d'|'7d'|'custom', startDate: string, endDate: string }>({
  data: { orderCount: 0, orderTotalValue: 0 },
  loading: false,
  rangeType: '1d',
  startDate: '',
  endDate: ''
})

const api = usepyStatisticsApi()

const computeRange = () => {
  const end = dayjs()
  if (state.rangeType === 'all') {
    state.startDate = ''
    state.endDate = ''
  } else if (state.rangeType === '1d') {
    state.startDate = end.format('YYYY-MM-DD')
    state.endDate = end.format('YYYY-MM-DD')
  } else if (state.rangeType === '3d') {
    state.startDate = end.subtract(2, 'day').format('YYYY-MM-DD')
    state.endDate = end.format('YYYY-MM-DD')
  } else if (state.rangeType === '7d') {
    state.startDate = end.subtract(6, 'day').format('YYYY-MM-DD')
    state.endDate = end.format('YYYY-MM-DD')
  }
}

const setPreset = (type: 'all'|'1d'|'3d'|'7d'|'custom') => {
  state.rangeType = type
  if (type !== 'custom') {
    computeRange()
    query()
  }
}

const onStartChange = (e: any) => {
  state.startDate = e.detail.value
}
const onEndChange = (e: any) => {
  state.endDate = e.detail.value
}

const query = async () => {
  state.loading = true
  uni.showLoading({ title: '加载中' })
  const params = { startDate: state.startDate, endDate: state.endDate }
  // 这里的调用使和系统自动捕获异常
  const res: any = await api.businessStatus(params, false, true)
  uni.hideLoading()
  if (res && res.result) {
    const r = res.result as any
    state.data.orderCount = r.orderCount || 0
    state.data.orderTotalValue = r.orderTotalValue || 0
  }
}

onMounted(() => {
  computeRange()
  query()
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

.refresh {
  position: absolute;
  right: 20rpx;
  top: 10rpx;
  height: 32rpx;
  line-height: 32rpx;
  color: $uni-color-primary;
  font-size: 26rpx;
}

.range {
  display: flex;
  align-items: center;
  gap: 20rpx;
  padding: 0 20rpx 10rpx 20rpx;
}
.range-item {
  padding: 10rpx 20rpx;
  border: 1px solid #ddd;
  border-radius: 8rpx;
  color: #666;
}
.range-item.active {
  border-color: $uni-color-primary;
  color: $uni-color-primary;
}

.custom-range {
  display: flex;
  align-items: center;
  gap: 20rpx;
  padding: 0 20rpx 10rpx 20rpx;
}
.date-bar { display:flex; align-items:center; gap: 20rpx; padding: 0 20rpx 10rpx 20rpx; }
.date-input {
  padding: 10rpx 20rpx;
  border: 1px solid #ddd;
  border-radius: 8rpx;
  color: #666;
}
.date-input.disabled { background: #f7f7f7; color: #999; }
.date-sep { color: #999; }

.query-btn {
  color: #fff;
  border-radius: 8rpx;
  background: $uni-color-primary;
  height: 60rpx;
  line-height: 60rpx;
  padding: 0 20rpx;
  font-size: 26rpx;
}

.card-body { display:flex; flex-wrap:wrap; }
.metric { width: 100%; padding: 20rpx; box-sizing: border-box; display:flex; align-items: baseline; justify-content: space-between; }
.metric-label { color:#999; font-size:26rpx; }
.metric-value { font-size: 40rpx; font-weight: bold; color:#333; }
.amount { color: $uni-color-primary; }
</style>
