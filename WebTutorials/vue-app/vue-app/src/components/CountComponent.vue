<script setup lang="ts">

import {onMounted} from "vue";

const props = defineProps<{ count: number }>()

onMounted(() => {
  console.log("CountComponent mounted " + props.count)
});

</script>

<template>
  <p>{{count}}</p>
</template>

<style scoped>

</style>