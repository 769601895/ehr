<template>
	<div style="flex: 1; background: #f3f4f6; overflow: auto">
		<el-container>
			<!-- 侧边栏 -->
			<el-aside v-show="!isFold" :class="isFold ? 'sidebar-fold' : 'expand-sidebar'" style="background: #f3f4f6; border-right: 1px solid #f0f0f0; display: flex; flex-direction: column">
				<div class="chat-action">
					<el-tooltip :content="$t('message.chat.newChat')" placement="top">
						<div class="chat-action-item" @click.stop="handleNewChat">
							<Edit style="width: 1.5em; height: 1.5em; color: #333" />
						</div>
					</el-tooltip>
					<el-tooltip :content="$t('message.chat.foldChat')" placement="top">
						<div class="chat-action-item" @click.stop="handleFoldChat">
							<Fold style="width: 1.5em; height: 1.5em; color: #333" />
						</div>
					</el-tooltip>
				</div>
				<div style="display: flex; flex-direction: column; align-items: center; padding-bottom: 8px; padding-top: 0px; margin-top: 0px">
					<el-avatar :size="60" style="background-color: #f3f4f6" src="/chat.png" fit="fill" class="avatar-with-shadow" />
					<div style="margin-top: 10px; font-weight: bold; font-size: 18px; color: #333">{{ $t('message.chat.title') }}</div>
				</div>
				<div style="flex: 1; overflow-y: auto; width: 100%">
					<div style="display: flex; flex-direction: column; gap: 12px; height: 100%">
						<Conversations
							style="border-radius: 0%; width: 255px"
							v-model:active="activeHistoryKey"
							:items="sideBarHistoryList"
							row-key="key"
							:show-tooltip="true"
							showToTopBtn
							:label-max-width="210"
							:load-more="loadMoreHistoryItems"
							:load-more-loading="isHistoryListLoading"
							showBuiltInMenu
							@change="handleChange"
						>
							<template #menu="{ item }">
								<div class="menu-buttons">
									<el-button v-for="menuItem in actionMenuItems" :key="menuItem.key" link size="default" @click="handleMenuClick(menuItem.key, item)">
										<el-icon v-if="menuItem.icon">
											<component :is="menuItem.icon" />
										</el-icon>
										<span v-if="menuItem.label">{{ menuItem.label }}</span>
									</el-button>
								</div>
							</template>
						</Conversations>
					</div>
				</div>
			</el-aside>

			<!-- 主体内容 -->
			<el-container>
				<el-main
					style="
						flex: 1;
						width: 100%;
						height: 100%;
						margin: 0;
						padding: 0;
						overflow-y: auto;
						display: flex;
						position: relative;
						flex-direction: column;
						align-items: center;
						justify-content: flex-start;
						background: #fff;
					"
				>
					<div class="main_action_toolbar" style="position: absolute; top: 0px; width: 500px; left: 20px">
						<div v-if="isFold" class="chat-action-item main_action_item">
							<el-tooltip :content="$t('message.chat.expandChat')" placement="top">
								<Expand style="width: 1.5em; height: 1.5em; color: #333" @click.stop="handleExpandChat" />
							</el-tooltip>
						</div>
						<div v-if="isFold" class="chat-action-item main_action_item" @click="handleNewChat">
							<el-tooltip :content="$t('message.chat.newChat')" placement="top">
								<Edit style="width: 1.5em; height: 1.5em; color: #333" />
							</el-tooltip>
						</div>
						<div v-if="canSwitchModel" class="model-select">
							<el-dropdown size="large">
								<span class="el-dropdown-link">
									{{ modellList.currentModel }}
									<el-icon class="el-icon--right">
										<arrow-down />
									</el-icon>
								</span>
								<template #dropdown>
									<el-dropdown-menu>
										<el-dropdown-item v-for="item in modellList.models" :key="item.modelName" @click="handleChangeModel(item)">
											<div class="model-item">
												<span>{{ item.providerName }}/{{ item.modelName }}</span>
												<el-icon v-if="item.modelName == modellList.currentModel" style="color: #409eff">
													<Select />
												</el-icon>
											</div>
										</el-dropdown-item>
									</el-dropdown-menu>
								</template>
							</el-dropdown>
						</div>
					</div>
					<div v-if="isNew" class="new_chat_title">
						<div style="margin-top: 60px; font-size: 36px; font-weight: bold; letter-spacing: 2px">Hello, {{ userName }}</div>
						<div style="margin-top: 20px; width: 100%; text-align: center; color: #999; font-size: 16px; font-weight: bold; letter-spacing: 2px">
							{{ $t('message.chat.subTitle') }}
						</div>
					</div>
					<div v-else class="chat_content">
						<BubbleList class="chat_content_list" ref="chatRef" :list="chatList" maxHeight="100%" style="padding: 0 60px 100px 60px" @complete="handleBubbleComplete" :triggerIndices="triggerIndices">
							<template #content="{ item }">
								<Typewriter
									v-if="chatList.length > 0 && chatList[chatList.length - 1].key === item.key && isSenderLoading"
									:content="item.content"
									:is-markdown="true"
									:typing="true"
									@finish="handleTypeingComplete"
								/>
								<XMarkdown v-else :markdown="item.content" default-theme-mode="light" class="markdown-body" />
							</template>
							<template #header="{ item }">
								<div v-if="item.role == 'assistant' && deepThinkingVisible && item.key == chatList[chatList.length - 1].key" class="header-wrapper">
									<Thinking max-width="100%" buttonWidth="250px" autoCollapse :content="deepThinkingMessage" :status="deepThinkingStatus" backgroundColor="#fff9e6" color="#000">
										<template #label="{ status }">
											<span v-if="status === 'start'">{{ $t('message.chat.startThinking') }}</span>
											<span v-else-if="status === 'thinking'">{{ $t('message.chat.thinking') }}</span>
											<span v-else-if="status === 'end'">{{ $t('message.chat.thinkingDone') }}</span>
											<span v-else-if="status === 'error'">{{ $t('message.chat.thinkingFailed') }}</span>
										</template>
										<template #content="{ content }">
											<span>
												<XMarkdown :markdown="content" default-theme-mode="light" class="vp-raw" />
											</span>
										</template>
									</Thinking>
								</div>
							</template>
							<template #footer="{ item }">
								<div v-if="item.role == 'assistant'" class="footer-container">
									<el-button type="info" text :icon="CopyDocument" size="small" @click="handleCopy(item)" />
									<el-button type="info" text size="small">
										<el-icon v-if="!isPlaying || playAudioKey != item.key" @click="handlePlay(item)">
											<VideoPlay />
										</el-icon>
										<el-icon v-if="isPlaying && playAudioKey == item.key" @click="handlePause(item)">
											<VideoPause />
										</el-icon>
									</el-button>
								</div>
							</template>
						</BubbleList>
					</div>
					<div :class="isNew ? 'chat_new_input_style' : 'chat_edit_input_style'">
						<sender
							ref="senderRef"
							variant="updown"
							clearable
							allow-speech
							:loading="isSenderLoading"
							:read-only="isSenderLoading"
							:auto-size="{ minRows: 1, maxRows: 5 }"
							v-model="senderInput"
							@submit="handleSend"
							:placeholder="$t('message.chat.inputPlaceholder')"
						>
							<template #prefix>
								<div style="display: flex; align-items: center; gap: 8px; flex-wrap: wrap">
									<el-button round plain>
										<el-icon>
											<Paperclip />
										</el-icon>
									</el-button>

									<div
										:class="{ isDeepThinking }"
										style="display: flex; align-items: center; gap: 4px; padding: 4px 12px; border: 1px solid silver; border-radius: 15px; cursor: pointer; font-size: 12px"
										@click="handleDeepThinking"
									>
										<el-icon>
											<ElementPlus />
										</el-icon>
										<span>{{ $t('message.chat.deepThinking') }}</span>
									</div>
								</div>
							</template>
						</sender>
					</div>
				</el-main>
			</el-container>
		</el-container>
		<!-- 自定义弹窗，添加动画和拖拽 -->
		<transition name="ai-modal-fade">
			<div v-if="renameDialogVisible" class="ai-rename-modal-mask" @mousedown.self="cancelRename">
				<div class="ai-rename-modal" ref="renameModalRef" @mousedown.stop>
					<div class="ai-rename-content">
						<div class="ai-rename-icon ai-rename-drag" @mousedown="startDrag">
							<el-avatar :size="48" src="/chat.png" />
						</div>
						<el-input v-model="renameInput" @keyup.enter="confirmRename" ref="renameInputRef" class="ai-rename-input" />
					</div>
					<div class="ai-dialog-footer">
						<el-button @click="cancelRename" class="ai-btn-cancel">{{ $t('message.chat.cancel') }}</el-button>
						<el-button type="primary" @click="confirmRename" class="ai-btn-confirm">
							{{ $t('message.chat.confirm') }}
						</el-button>
					</div>
				</div>
			</div>
		</transition>
	</div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, markRaw, nextTick, watch } from 'vue';
import { Edit, Fold, Expand, ArrowDown, Select, Delete, CopyDocument, Share, Paperclip, ElementPlus, VideoPlay, VideoPause } from '@element-plus/icons-vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { userFriendName, userId } from '/@/utils/useInfo';
import { useI18n } from 'vue-i18n';

import { franc } from 'franc';
import { v4 as uuidv4 } from 'uuid';
import { BubbleList, Conversations, Sender, Thinking, Typewriter, XMarkdown } from 'vue-element-plus-x';
import type { BubbleListItemProps, BubbleListProps } from 'vue-element-plus-x/types/components/BubbleList/types';
import type { ConversationItem } from 'vue-element-plus-x/types/components/Conversations/types';
import type { TypewriterInstance } from 'vue-element-plus-x/types/components/Typewriter/types';

import { getAPI } from '/@/utils/axios-utils';
import { LLMChatApi } from '/@/api-services/aiChat/api';
import { ModelListOutput, ModelListOutputItem, LLMChatHistory, LLMChatSummaryHistory, ChatInput, ChatListInput, ChatOutput, ChatListOutput } from '/@/api-services/aiChat/models';

const { t } = useI18n();
const canSwitchModel = ref(false);
const userName = userFriendName();
const isNew = ref(true);
const senderRef = ref<any>(null);
const isDeepThinking = ref(false);
const isFold = ref(false);
const sidebarWidth = ref(260);
const isPlaying = ref(false);
const playAudioKey = ref('');
const chatRef = ref<any>();
const sideBarHistoryList = ref<ConversationItem<{ key: string; label: string }>[]>([]);
type listType = BubbleListItemProps & {
	key: string;
	role: 'user' | 'assistant';
};
const state = ref<ChatListInput & { totalPages: number }>({
	page: 1,
	pageSize: 10,
	totalPages: 0,
});

const isHistoryListLoading = ref(false); // 历史记录加载更多处理

const activeHistoryKey = ref('');
const chatList = ref<BubbleListProps<listType>['list']>([]);
const currentChatItemMessage = ref(''); // 临时存储当前聊天内容
const currentChatItem = ref<LLMChatHistory>({} as LLMChatHistory);
const isSenderLoading = ref(false);

const deepThinkingMessage = ref('');
const deepThinkingStatus = ref('start');
const deepThinkingVisible = ref(false);

let historyListSource: ChatListOutput[] = []; //从后台获取的原始历史记录

let utterance: SpeechSynthesisUtterance | null = null;
const triggerIndices = ref<BubbleListProps['triggerIndices']>('only-last');

const senderInput = ref('');
const modellList = ref<ModelListOutput>({
	models: [],
	providerName: '',
	currentModel: '',
});
const actionMenuItems = [
	// 侧边栏历史记录操作菜单
	{
		key: 'rename',
		label: t('message.chat.rename'),
		icon: Edit,
	},
	{
		key: 'delete',
		label: t('message.chat.delete'),
		icon: Delete,
	},
];
// 处理菜单点击
const renameDialogVisible = ref(false);
const currentEditItem = ref<any>(null);
const renameInput = ref('');
const renameModalRef = ref<HTMLElement | null>(null);
let dragData = { dragging: false, offsetX: 0, offsetY: 0 };

//#region sse客户端
let eventSource: EventSource | null = null;
let monitorSSEConnectionHandler: NodeJS.Timeout | null = null;
let lastSseConnectionTime = Date.now();
let isSSEConnectionClosed = false;
const SSE_CONNECTION_TIMEOUT = 5000;
// 初始化sse连接
const initSSEConnection = () => {
	initSSEConnectionCore();
	// 监控sse连接
	monitorSSEConnectionHandler = setInterval(() => {
		isSSEConnectionClosed = Date.now() - lastSseConnectionTime > SSE_CONNECTION_TIMEOUT;
		if (isSSEConnectionClosed) {
			console.log('SSE connection timed out, reconnecting');
			try {
				initSSEConnectionCore();
			} catch (err) {
				console.log('SSE connection timed out, reconnecting failed', err);
			}
		}
	}, SSE_CONNECTION_TIMEOUT);
};

// 检查sse连接状态
const checkSSEConnectionStatus = () => {
	if (isSSEConnectionClosed) {
		return false;
	}
	return true;
};
// 初始化sse连接核心代码
const initSSEConnectionCore = () => {
	closeSSEConnection();

	eventSource = new EventSource('/sse/chat/' + userId());

	// 收到deepThinking消息
	eventSource.addEventListener('deepThinking', (event) => {
		let data = event.data?.replace(/\\x0A/g, '\n');
		if (data?.includes('[BEGIN]')) {
			deepThinkingMessage.value = '';
			deepThinkingStatus.value = 'thinking';
			return;
		}
		if (data?.includes('[DONE]')) {
			deepThinkingMessage.value = deepThinkingMessage.value;
			deepThinkingStatus.value = 'end';
			return;
		}
		deepThinkingMessage.value = deepThinkingMessage.value + data;
	});

	// 收到chat消息
	eventSource.addEventListener('chat', (event) => {
		let data = event.data?.replace(/\\x0A/g, '\n');
		if (data?.includes('[BEGIN]')) {
			currentChatItemMessage.value = '';
			currentChatItem.value.content = '';
			chatList.value[chatList.value.length - 1].content = '';
			chatList.value[chatList.value.length - 1].loading = true;
			isSenderLoading.value = true;
			return;
		}
		if (data?.includes('[DONE]')) {
			currentChatItem.value.content = currentChatItemMessage.value;
			chatList.value[chatList.value.length - 1].loading = false;
			chatList.value[chatList.value.length - 1].isMarkdown = true;
			chatList.value[chatList.value.length - 1].content = currentChatItemMessage.value;
			chatRef?.value?.scrollToBottom();
			return;
		}
		currentChatItemMessage.value = currentChatItemMessage.value + data; // 先接收流式数据存放在临时变量中
	});

	// 收到ping消息, 用于心跳检测
	eventSource.addEventListener('ping', (event) => {
		console.log('heat beat:', event.data);
		lastSseConnectionTime = Date.now();
	});

	eventSource.onerror = () => {
		console.log('SSE connection error');
	};

	eventSource.onopen = (event) => {
		console.log('SSE connection opened:', event);
	};
};

const closeSSEConnection = () => {
	if (eventSource) {
		eventSource.close();
		eventSource = null;
	}
};

const handleBubbleComplete = (instance: TypewriterInstance, index: number) => {
	isSenderLoading.value = false;
	chatRef?.value?.scrollToBottom();
};

const handleTypeingComplete = () => {
	isSenderLoading.value = false;
	chatRef?.value?.scrollToBottom();
};

//#endregion sse客户端
const handleSend = async () => {
	if (!senderInput.value.trim()) return;
	if (!checkSSEConnectionStatus()) {
		ElMessage.error(t('message.chat.backEndError'));
		return;
	}
	isSenderLoading.value = true;
	currentChatItemMessage.value = '';
	if (isNew.value) {
		//新建聊天
		isNew.value = false;
		nextTick(async () => {
			await newChat();
		});
	} else {
		//续聊
		await continueChat();
	}
};

//新建聊天
const newChat = async () => {
	let inputStr = senderInput.value;
	senderInput.value = '';
	currentChatItemMessage.value = '';
	let maxId = historyListSource.reduce((max, item) => Math.max(max, item.id || 0), 0);
	maxId++;
	let chatSummary: LLMChatSummaryHistory = {
		id: maxId,
		userId: userId(),
		summary: 'New Chat',
		uniqueToken: 'add_new_chat',
		utcCreateTime: new Date().getTime(),
		histories: [],
	} as LLMChatSummaryHistory;
	let userChat: LLMChatHistory = {
		id: 0,
		summaryId: chatSummary.id,
		role: 'user',
		content: inputStr,
		userId: userId(),
		utcCreateTime: new Date().getTime(),
	} as LLMChatHistory;
	chatSummary.histories?.push(userChat);
	chatList.value = [] as BubbleListProps<listType>['list'];
	addChatItem(userChat, false);
	let assistantChat: LLMChatHistory = {
		id: 0,
		summaryId: chatSummary.id,
		role: 'assistant',
		content: '',
		userId: userId(),
		utcCreateTime: new Date().getTime(),
	} as LLMChatHistory;
	chatSummary.histories?.push(assistantChat);
	addChatItem(assistantChat, true);
	currentChatItem.value = assistantChat;
	if (isDeepThinking.value) {
		deepThinkingVisible.value = true;
		deepThinkingStatus.value = 'start';
		deepThinkingMessage.value = t('message.chat.thinkingPrepare');
	}
	chatRef?.value?.scrollToBottom();
	// 添加到历史记录列表
	historyListSource.push(chatSummary);
	sideBarHistoryList.value.unshift({
		key: chatSummary.uniqueToken || '',
		label: chatSummary.summary || '',
	});
	activeHistoryKey.value = chatSummary.uniqueToken || '';
	let resultData = await getAPI(LLMChatApi).apiLLMChatChatPost({
		uniqueToken: chatSummary.uniqueToken || '',
		message: inputStr,
		providerName: modellList.value.providerName,
		modelId: modellList.value.currentModel,
		summaryId: chatSummary.id,
		summary: chatSummary.summary || '',
		deepThinking: isDeepThinking.value,
	});
	let result = resultData.data?.result || ({} as ChatOutput);
	// 更新摘要与侧边栏聊天数据
	chatSummary.summary = result.summary;
	chatSummary.uniqueToken = result.uniqueToken;
	let sideBarItem = sideBarHistoryList.value.find((u) => u.key == 'add_new_chat');
	if (sideBarItem) {
		sideBarItem.label = result.summary || '';
		sideBarItem.key = result.uniqueToken || '';
	}
	activeHistoryKey.value = result.uniqueToken || '';
};

//续聊：已经有自己的摘要与聊天记录
const continueChat = async () => {
	if (!activeHistoryKey.value) return;
	isSenderLoading.value = true;
	let inputStr = senderInput.value;
	senderInput.value = '';
	let currentHistoryItem = historyListSource.find((u) => u.uniqueToken == activeHistoryKey.value); // 获取当前聊天的记录与摘要
	if (!currentHistoryItem) return;
	let list = currentHistoryItem.histories;
	let userChat: LLMChatHistory = {
		id: 0,
		summaryId: currentHistoryItem.id,
		role: 'user',
		content: inputStr,
		summary: currentHistoryItem as LLMChatSummaryHistory,
		userId: currentHistoryItem.userId,
		utcCreateTime: new Date().getTime(),
	} as LLMChatHistory;
	list?.push(userChat);
	addChatItem(userChat, false);
	let assistantChat: LLMChatHistory = {
		id: 0,
		summaryId: currentHistoryItem.id,
		role: 'assistant',
		content: '',
		summary: currentHistoryItem as LLMChatSummaryHistory,
		userId: currentHistoryItem.userId,
		utcCreateTime: new Date().getTime(),
	} as LLMChatHistory;
	list?.push(assistantChat);
	currentChatItem.value = assistantChat;
	addChatItem(assistantChat, true);
	if (isDeepThinking.value) {
		deepThinkingVisible.value = true;
		deepThinkingStatus.value = 'start';
		deepThinkingMessage.value = t('message.chat.thinkingPrepare');
	}

	chatRef?.value?.scrollToBottom();
	let resultData = await getAPI(LLMChatApi).apiLLMChatChatPost({
		uniqueToken: currentHistoryItem.uniqueToken,
		message: inputStr,
		providerName: modellList.value.providerName,
		modelId: modellList.value.currentModel,
		summaryId: currentHistoryItem.id,
		summary: currentHistoryItem.summary,
		deepThinking: isDeepThinking.value,
	});
	let result = resultData.data?.result || ({} as ChatOutput);
	currentHistoryItem.summary = result.summary;
	if (currentHistoryItem.id != result.summaryId) {
		currentHistoryItem.id = result.summaryId || currentHistoryItem.id;
	}
	let sideBarItem = sideBarHistoryList.value.find((u) => u.key == currentHistoryItem.uniqueToken);
	if (sideBarItem) {
		sideBarItem.label = result.summary || '';
	}
	senderInput.value = '';
};

// 深度思考
const handleDeepThinking = () => {
	isDeepThinking.value = !isDeepThinking.value;
	if (!isDeepThinking.value) {
		deepThinkingVisible.value = false;
	}
};

// 新建聊天
const handleNewChat = () => {
	if (isSenderLoading.value) return;
	isNew.value = true;
	chatList.value.length = 0;
};

// 折叠侧边栏
const handleFoldChat = () => {
	isFold.value = true;
	sidebarWidth.value = isFold.value ? 0 : 260;
};

// 展开侧边栏
const handleExpandChat = () => {
	isFold.value = false;
	sidebarWidth.value = isFold.value ? 0 : 260;
};

// 切换模型
const handleChangeModel = async (item: ModelListOutputItem) => {
	let res = await getAPI(LLMChatApi).apiLLMChatChangeModelPost({
		modelName: item.modelName,
		providerName: item.providerName,
	});
	if (res.data?.result) {
		modellList.value.currentModel = item.modelName;
	} else {
		ElMessage.error(t('message.chat.changeModelError'));
	}
};

// 切换历史记录
const handleChange = (item: ConversationItem<{ key: string; label: string }>) => {
	if (isSenderLoading.value) return;
	activeHistoryKey.value = item.key;
	isNew.value = false;
	chatList.value.length = 0;
	let currentHistoryItem = historyListSource.find((u) => u.uniqueToken == item.key);
	if (currentHistoryItem) {
		let list = currentHistoryItem.histories;
		list = list?.filter((u: LLMChatHistory) => u.role == 'user' || u.role == 'assistant');
		list?.forEach((u: LLMChatHistory) => addChatItem(u));
	}
	chatRef?.value?.scrollToBottom();
};

// 添加聊天记录
const addChatItem = (chatItem: LLMChatHistory, typing: boolean = false) => {
	const placement: 'end' | 'start' = chatItem.role == 'user' ? 'end' : 'start';
	const isMarkdown = chatItem.role == 'user' ? false : true;
	const maxWidth = chatItem.role == 'user' ? '500px' : '100%';
	const noStyle = chatItem.role == 'user' ? false : true;
	let addRow = {
		role: chatItem.role as 'user' | 'assistant',
		content: chatItem.content || '',
		key: uuidv4(),
		placement: placement,
		isMarkdown: isMarkdown,
		maxWidth: maxWidth,
		noStyle: noStyle,
		loading: typing,
		typing,
	};
	chatList.value.push(addRow);
};

// 复制
const handleCopy = (item: BubbleListItemProps) => {
	const textToCopy = item.content || '';
	if (navigator.clipboard) {
		navigator.clipboard
			.writeText(textToCopy)
			.then(() => {
				ElMessage.success(t('message.chat.copySuccess'));
			})
			.catch((err) => {
				ElMessage.error(t('message.chat.copyError'));
			});
	} else {
		ElMessage.error(t('message.chat.browerNotSupport'));
	}
};

// 播放语音
const handlePlay = (item: any) => {
	console.log('handlePlay:', item.key);
	isPlaying.value = true;
	playAudioKey.value = item.key;
	if (utterance) {
		window.speechSynthesis.cancel();
	}
	utterance = new SpeechSynthesisUtterance(item.content);
	const lang = franc(item.content || '');
	utterance.lang = lang;
	utterance.onend = () => {
		isPlaying.value = false;
	};
	window.speechSynthesis.speak(utterance);
};

// 暂停语音
const handlePause = (item: BubbleListItemProps) => {
	isPlaying.value = false;
	if (utterance) {
		window.speechSynthesis.cancel();
	}
};

//侧边栏历史记录加载更多
const loadMoreHistoryItems = async () => {
	if (isHistoryListLoading.value) return;
	state.value.page = (state.value.page || 0) + 1;
	if (state.value.page > state.value.totalPages) {
		state.value.page = state.value.totalPages;
		return;
	}
	isHistoryListLoading.value = true;
	await getHistoryList();
	isHistoryListLoading.value = false;
};

const handleMenuClick = (menuKey: string, item: any) => {
	switch (menuKey) {
		case 'rename':
			currentEditItem.value = item;
			renameInput.value = item.label;
			renameDialogVisible.value = true;
			break;
		case 'delete':
			deleteHistoryItem(item);
			break;
		default:
			break;
	}
};

const deleteHistoryItem = async (item: any) => {
	await ElMessageBox.confirm(t('message.chat.confirmDeleteHistoryItem', { historyItem: item.label }), 'warning', {
		confirmButtonText: t('message.chat.confirm'),
		cancelButtonText: t('message.chat.cancel'),
	});
	let currentItem = historyListSource.find((u) => u.uniqueToken == item.key);
	if (currentItem) {
		let res = await getAPI(LLMChatApi).apiLLMChatDeleteSummaryAllPost({
			uniqueToken: currentItem.uniqueToken,
			summaryId: currentItem.id,
		});
		if (res.data?.result) {
			historyListSource.splice(historyListSource.indexOf(currentItem), 1);
			sideBarHistoryList.value = sideBarHistoryList.value.filter((u) => u.key != currentItem.uniqueToken);
			isNew.value = true;
		}
	} else {
		ElMessage.error(t('message.chat.deleteHistoryItemError'));
	}
};

// 重命名的方法
async function confirmRename() {
	if (currentEditItem.value && renameInput.value.trim()) {
		if (currentEditItem.value.label != renameInput.value.trim()) {
			let currentItem = historyListSource.find((item) => item.uniqueToken == currentEditItem.value.key);
			if (currentItem) {
				currentItem.summary = renameInput.value.trim();
				await getAPI(LLMChatApi).apiLLMChatRenameSummaryLablePost({
					uniqueToken: currentItem.uniqueToken,
					summary: renameInput.value.trim(),
					summaryId: currentItem.id,
					message: '',
					providerName: modellList.value.providerName,
					modelId: modellList.value.currentModel,
				});
			}
			let leftCurrentItem = sideBarHistoryList.value.find((item) => item.key == currentEditItem.value.key);
			if (leftCurrentItem) {
				leftCurrentItem.label = renameInput.value.trim();
			}
			currentEditItem.value = null;
			renameInput.value = '';
		}
		renameDialogVisible.value = false;
	} else {
		ElMessage.error(t('message.chat.titleCannotBeEmpty'));
	}
}

// 添加取消重命名的方法
const cancelRename = () => {
	renameDialogVisible.value = false;
	currentEditItem.value = null;
	renameInput.value = '';
};

//分页获取历史记录
async function getHistoryList() {
	let res = await getAPI(LLMChatApi).apiLLMChatChatListPost(state.value);
	historyListSource = [...historyListSource, ...(res.data?.result?.items || ([] as ChatListOutput[]))];
	state.value.totalPages = res.data?.result?.totalPages || 0;
	sideBarHistoryList.value = [
		...sideBarHistoryList.value,
		...historyListSource.map((item: ChatListOutput) => {
			return {
				key: item.uniqueToken || '',
				label: item.summary || '',
			} as { key: string; label: string };
		}),
	];
}

//#region 自定义弹窗口
function startDrag(e: MouseEvent) {
	if (!renameModalRef.value) return;
	dragData.dragging = true;
	const modal = renameModalRef.value;
	const rect = modal.getBoundingClientRect();
	dragData.offsetX = e.clientX - rect.left;
	dragData.offsetY = e.clientY - rect.top;
	document.addEventListener('mousemove', onDragMove);
	document.addEventListener('mouseup', stopDrag);
}

function onDragMove(e: MouseEvent) {
	if (!dragData.dragging || !renameModalRef.value) return;
	const modal = renameModalRef.value;
	let left = e.clientX - dragData.offsetX;
	let top = e.clientY - dragData.offsetY;
	// 限制弹窗不出屏幕
	const minLeft = 0,
		minTop = 0;
	const maxLeft = window.innerWidth - modal.offsetWidth;
	const maxTop = window.innerHeight - modal.offsetHeight;
	left = Math.max(minLeft, Math.min(left, maxLeft));
	top = Math.max(minTop, Math.min(top, maxTop));
	modal.style.left = left + 'px';
	modal.style.top = top + 'px';
	modal.style.margin = '0';
	modal.style.position = 'fixed';
}

function stopDrag() {
	dragData.dragging = false;
	document.removeEventListener('mousemove', onDragMove);
	document.removeEventListener('mouseup', stopDrag);
}

// 弹窗初始居中
watch(
	() => renameDialogVisible.value,
	(val) => {
		if (val && renameModalRef.value) {
			nextTick(() => {
				const modal = renameModalRef.value;
				if (modal) {
					modal.style.left = '';
					modal.style.top = '';
					modal.style.margin = '';
					modal.style.position = '';
				}
			});
		}
	}
);
// #endregion 自定义弹窗口

onMounted(async () => {
	initSSEConnection();
	let res = await getAPI(LLMChatApi).apiLLMChatModelListGet();
	modellList.value = res.data?.result || {};
	canSwitchModel.value = res.data?.result?.userCanSwitchLLM || false;
	if (!modellList.value.models || modellList.value.models.length == 0) {
		ElMessage.error(t('message.chat.fetchModelError'));
		return;
	}
	await getHistoryList();
});

onBeforeUnmount(() => {
	closeSSEConnection();
	if (monitorSSEConnectionHandler) {
		clearInterval(monitorSSEConnectionHandler);
		monitorSSEConnectionHandler = null;
	}
	if (utterance) {
		window.speechSynthesis.cancel();
		utterance = null;
	}
});
</script>

<style scoped lang="less">
.chat-action {
	display: flex;
	flex-direction: row;
	justify-content: space-between;
	align-items: center;
	padding: 0 16px;
	height: 48px;
}

.el-menu-vertical-demo {
	border: none;
}

.fill-parent,
.el-container,
.el-main {
	height: 100%;
	min-height: 0;
}

.avatar-with-shadow {
	box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
	transition: all 0.3s ease;
}

.avatar-with-shadow:hover {
	box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
	transform: translateY(-2px);
	cursor: pointer;
}

.chat-action-item {
	display: flex;
	flex-direction: row;
	justify-content: center;
	align-items: center;
	border-radius: 4px;
	text-align: center;
	height: 2em;
	width: 2em;
	background-color: #f3f4f6;
	cursor: pointer;
}

.chat-action-item:hover {
	background-color: #e5e7eb;
}

.main_action_item {
	background-color: #ffffff;
}

.main_action_toolbar {
	display: flex;
	flex-direction: row;
	justify-content: flex-start;
	align-items: center;
	gap: 10px;
	height: 48px;
}

.sidebar-fold {
	width: 0;
}

.expand-sidebar {
	width: 260px;
}

.example-showcase .el-dropdown-link {
	cursor: pointer;
	color: var(--el-color-primary);
	display: flex;
	align-items: center;
}

.model-select {
	display: flex;
	flex-direction: row;
	justify-content: center;
	align-items: center;
	font-size: 18px !important;
	font-weight: bold;
	color: #333;
	font-family: 'PingFang SC', 'Microsoft YaHei', sans-serif;
	cursor: pointer;
}

.model-item {
	display: flex;
	flex-direction: row;
	justify-content: space-between;
	align-items: center;
	font-size: 14px;
	font-weight: bold;
	color: #333;
	font-family: 'PingFang SC', 'Microsoft YaHei', sans-serif;
	width: 240px;
}

.menu-buttons {
	display: flex;
	flex-direction: column;
	gap: 8px;
	align-items: flex-start;
	padding: 12px;

	// 自定义菜单按钮-el-button样式
	.el-button {
		padding: 4px 8px;
		margin-left: 0;
		text-align: left;

		.el-icon {
			margin-right: 8px;
		}
	}
}

.ai-modal-fade-enter-active,
.ai-modal-fade-leave-active {
	transition:
		opacity 0.18s cubic-bezier(0.55, 0, 0.1, 1),
		transform 0.18s cubic-bezier(0.55, 0, 0.1, 1);
}

.ai-modal-fade-enter-from,
.ai-modal-fade-leave-to {
	opacity: 0;
	transform: scale(0.96);
}

.ai-modal-fade-enter-to,
.ai-modal-fade-leave-from {
	opacity: 1;
	transform: scale(1);
}

.ai-rename-modal-mask {
	position: fixed;
	z-index: 3000;
	left: 0;
	top: 0;
	width: 100vw;
	height: 100vh;
	background: rgba(0, 0, 0, 0.18);
	display: flex;
	align-items: center;
	justify-content: center;
}

.ai-rename-modal {
	background: #fff;
	border-radius: 20px;
	box-shadow: 0 8px 32px rgba(36, 120, 255, 0.1);
	min-width: 320px;
	max-width: 90vw;
	padding: 32px 24px 18px 24px;
	display: flex;
	flex-direction: column;
	align-items: center;
	position: relative;
	/* 拖拽后会变成fixed */
}

.ai-rename-drag {
	cursor: move;
}

.ai-rename-content {
	display: flex;
	flex-direction: column;
	align-items: center;
	gap: 18px;
	width: 100%;
}

.ai-rename-icon {
	margin-bottom: 8px;
}

.ai-rename-input {
	width: 260px;
	height: 44px;

	:deep(.el-input__wrapper) {
		border-radius: 22px;
		font-size: 18px;
		padding: 0 18px;
		box-shadow: 0 2px 12px rgba(36, 120, 255, 0.08);
		background: #f7faff;
	}
}

.ai-dialog-footer {
	display: flex;
	justify-content: center;
	gap: 18px;
	width: 100%;
	margin-top: 15px;

	.el-button {
		border-radius: 20px;
		font-size: 16px;
		padding: 8px 28px;
		font-weight: 500;

		&.ai-btn-cancel {
			background: #f7faff;
			color: #409eff;
			border: none;
		}

		&.ai-btn-confirm {
			background: linear-gradient(90deg, #409eff 0%, #36d1dc 100%);
			color: #fff;
			border: none;
		}
	}
}

.new_chat_title {
	display: flex;
	flex-direction: column;
	align-items: center;
	justify-content: center;
	width: 90%;
}

.chat_content {
	width: 100%;
	margin-top: 50px;
	flex: 1;
	height: 100%;
	background-color: #fff;
	overflow-y: auto;
}

.isDeepThinking {
	color: #626aef;
	border: 1px solid #626aef !important;
	border-radius: 15px;
	padding: 3px 12px;
	font-weight: 700;
}

.chat_new_input_style {
	width: calc(100% - 120px);
	position: static;
	margin-top: 30px;
	padding: 0;
	background-color: #fff;
}

.chat_edit_input_style {
	width: calc(100% - 120px);
	position: absolute;
	bottom: 0;
	background-color: #fff;
	// display:none;
}

.chat_content_list :deep(.typer-content) {
	font-size: 16px !important;
	letter-spacing: 0.03em !important;
	line-height: 2em !important;
	font-weight: 500 !important;
}
.vp-raw {
	background-color: #f0f0f0;
	padding: 2px 20px;
	border-radius: 4px;
	line-height: 2em;
}
.markdown-body {
	font-size: 16px !important;
	letter-spacing: 0.03em !important;
	line-height: 2em !important;
	font-weight: 500 !important;
}
</style>
