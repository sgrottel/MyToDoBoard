function removeLink(sym) {
	const symNode = document.getElementById(sym);
	symNode.innerText = symNode.innerText.trim();
}

function sgrlhiopCall(msg, callback) {
	let callbackId = "MyToDoHtml-" + crypto.randomUUID();
	let invoker = document.getElementById("invoker");
	invoker.src = `sgrlhiop:${callbackId}:${msg}`;
	let callbackReceiver = new CallbackReceiver(callbackId, localHtmlInteropPort);
	callbackReceiver.setOnResultCallback(callback);
}

function installLink(sym, cbfunc) {
	const symNode = document.getElementById(sym);
	const txt = symNode.innerText.trim();
	while (symNode.firstChild) {
		symNode.removeChild(symNode.firstChild);
	}
	const link = document.createElement("a");
	symNode.appendChild(link);
	link.innerText = txt;
	link.onclick = cbfunc;
}

function startCheck() {
	removeLink('lHtmlIopState');
	removeLink('lHtmlIopUpdate');
	installLink('lHtmlIopUpdate', startUpdate);
	removeLink('lHtmlIopEdit');
	installLink('lHtmlIopEdit', startEdit);
	removeLink('lHtmlIopExplore');
	installLink('lHtmlIopExplore', startExplore);

	const symNode = document.getElementById('lHtmlIopState');
	symNode.innerText = '❔';
	symNode.setAttribute('title', 'Evaluating...');
	symNode.classList.add('evaluating');
	setTimeout(function() {
		sgrlhiopCall(
			`mytodo/check?f=${encodeURIComponent(localHtmlInteropF)}&t=${encodeURIComponent(localHtmlInteropT)}`,
			function(r) {
				if (r.status === 'completed') {
					if (r.exitcode !== 0) {
						r.status = 'error';
					} else {
						const i = parseInt(r.output);
						if (i === 0) {
							symNode.classList.remove('evaluating');
							symNode.innerText = '⚠️';
							symNode.setAttribute('title', 'Report is outdated. Consider recreation.');
						} else if (i === 1) {
							symNode.classList.remove('evaluating');
							symNode.innerText = '✅';
							symNode.setAttribute('title', 'Report is up to date.');
						} else {
							r.status = 'error';
						}
					}
				}
				if (r.status === 'error') {
					symNode.classList.remove('evaluating');
					symNode.classList.add('error');
					symNode.innerText = '❌';
					symNode.setAttribute('title', `Error: ${JSON.stringify(r)}`);
				}
			});
		}, 200);
}

function startEdit()
{
	removeLink('lHtmlIopEdit');
	const symNode = document.getElementById('lHtmlIopEdit');
	symNode.classList.remove('error');
	symNode.classList.add('evaluating');
	sgrlhiopCall(
		`mytodo/edit?f=${encodeURIComponent(localHtmlInteropF)}`,
		function(r) {
			if (r.status === 'completed') {
				if (r.exitcode !== 0) {
					r.status = 'error';
				} else {
					symNode.classList.remove('evaluating');
					installLink('lHtmlIopEdit', startEdit);
				}
			}
			if (r.status === 'error') {
				symNode.classList.remove('evaluating');
				symNode.classList.add('error');
				symNode.setAttribute('title', `Error: ${JSON.stringify(r)}`);
				installLink('lHtmlIopEdit', startEdit);
			}
		});
}

function startExplore()
{
	removeLink('lHtmlIopExplore');
	const symNode = document.getElementById('lHtmlIopExplore');
	symNode.classList.remove('error');
	symNode.classList.add('evaluating');
	sgrlhiopCall(
		`mytodo/browse?f=${encodeURIComponent(localHtmlInteropF)}`,
		function(r) {
			if (r.status === 'completed') {
				if (r.exitcode !== 0) {
					r.status = 'error';
				} else {
					symNode.classList.remove('evaluating');
					installLink('lHtmlIopExplore', startExplore);
				}
			}
			if (r.status === 'error') {
				symNode.classList.remove('evaluating');
				symNode.classList.add('error');
				symNode.setAttribute('title', `Error: ${JSON.stringify(r)}`);
				installLink('lHtmlIopExplore', startExplore);
			}
		});
}

function startUpdate()
{
	removeLink('lHtmlIopUpdate');
	const symNode = document.getElementById('lHtmlIopUpdate');
	symNode.classList.remove('error');
	symNode.classList.add('evaluating');
	sgrlhiopCall(
		`mytodo/update?f=${encodeURIComponent(localHtmlInteropF)}&t=${encodeURIComponent(localHtmlInteropT)}`,
		function(r) {
			if (r.status === 'completed') {
				if (r.exitcode !== 0) {
					r.status = 'error';
				} else {
					symNode.classList.remove('evaluating');
					installLink('lHtmlIopUpdate', startUpdate);
					setTimeout(function() {
						location. reload();
					}, 1000);
				}
			}
			if (r.status === 'error') {
				symNode.classList.remove('evaluating');
				symNode.classList.add('error');
				symNode.setAttribute('title', `Error: ${JSON.stringify(r)}`);
				installLink('lHtmlIopUpdate', startUpdate);
			}
		});
}
