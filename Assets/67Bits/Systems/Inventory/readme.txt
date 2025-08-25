READ-ME do sistema de Inventário
sendo editado por: Vitor Balbino
		
-----------------------

Casos de uso

	O sistema de inventário deve resolver os seguintes casos:

	- Loja na UI, ou uma UI de upgrades  
	- Troca (estilo arcade idle) no espaço do jogo  

	- Colocar player inventory na UI deveria ser apenas arrastar um prefab de UI pro jogo.  

	- Para um personagem representar um inventory, deveria bastar colocar no personagem um InventoryCarryer.  
		- Todo InventoryCarryer deveria ser detectável.  
		- Minions que carregassem itens poderiam ter detecção de inventário.  

	- Item drop (Será outro sistema, mas da parte dos itens, os scripts devem estar prontos.)  
		- Quando o personagem do jogador se mover sobre o item, ele deve coletar  

	- Item Deposit  
		- Minions vão carregar itens coletados e depositá-los em algum lugar.  

	- (sistema opcional) Itens poderem ser coletados com cliques na tela sobre eles.

	- A UI do jogador deve ter alguns itens sempre visíveis e outros só se estiverem em uso  
		- Se ocultam se o jogador não estiver usando
		- Com um botão de abrir e fechar a lista toda.

	- O item deve aparecer na lista se for coletado.  

	- Jogador pode ganhar itens através de um minigame (como o Train Gold Rush)  
	- Jogador pode coletar itens através de alguma ação do jogo (como em um tutorial)
	
	- Ser fácil de saber quanto itens o jogador têm por código
	- Ser fácil de achar depósitos, e indicã-los na UI

-----------------------

O que produzir: 

--- UI simples:

	- Script "CollectItem" : Monobehavior  
		- Usado para ser adicionado num UnityEvent e assim entregar itens ao jogador 
		- Tem informações de quantos itens o jogador ganha.

	- UI Prefab (Canvas de) lista de itens (de um inventário, pq no caso do Zombie, tem duas listas na tela)  
		- Esse prefab já deve vir montado 
		- Deve ser fácil visualmente de editar (trocar lista de itens, trocar pose) 
		- Seria útil que alguns itens ficassem desligados.
		- Seria útil que a UI pudesse automaticamente se organizar para certas animações.
			- Esconder contadores menos recorrentes
			- Mostrar contador para certas animações, como coleta animada na UI.

	- Sistema de animação de UI de item coletado (viajando de um ponto na tela para a UI)
		- Valores demoram um pouco para serem acumulados na UI, mas já seriam acumulados no jogo.
		- O slot da lista aparece para quando o item for coletado.

	- UI Prefab "Purchase button"  
		- Tem que receber um item  
		- Fica desligado se o jogador não tem o valor devido  
		- Fica ligado se o jogador tem o valor devido  
		- Atualiza a cada atualização do valor  

--- Coletáveis:

	- 3D Prefab Item (como o cérebro no Alien)  
		- Já tem toda a animação do item  
		- Basta só a movimentação e o sprite certos  
		- Indica ao agente qual item é e checa quem coleta.  

--- Arcade:

	- 3D UI Prefab "Item Count"  
		- Pra ser usado no jogo, como um balão  
		- Usado em qualquer lugar que precise mostrar em jogo o item.  
		- Deve mostrar o ícone do jogo e o item carregado  
		? - Pode mostrar mais itens?  

	- Script "ItemCarrier" : Monobehavior 
		- Agente com inventário pessoal.
		- Determina que um agente é coletor do item  
		- Fica pré determinado qual item coleta e até quanto.  
		- Tem UI própria de mostrar itens sendo carregados.  
		- Tanto um minion coletor, quanto um depósito ou um baú usam.  
		? - O jogador tem, e representa a UI do jogo.  
		- Ajuda a generalizar os outros scripts.
	
	- flag enum CarrierType  
		- Player, store, deposit, anyAlly, anyEnemy  
		- Ajuda a generalizar os outros scripts.  
		 
	- Script "Item deposit" : Monobehavior
		- Um depósito passivo, onde o agente deposita / retira seus itens.
		- Onde um agente (como um minion ou um gerador de itens) deposita itens para serem buscados.  
		- Talvez tenha uma lista de tags para quem coleta o quê 
			
	- 3D Prefab Arcade exchange  
		- Quando um agente com inventário é detectado, o exchange ativa.  
		- Ao ser ativo, aparece o preço do item  
		- Enquanto ativo, o contador do item carrega, dando a entender a confirmação da compra  
		- Ao ser concluído, o valor da compra é processado.

--- Clicker:  

	? - UI Prefab "PlayerClickable"  
		- Caso o jogador clique nele, o item invoca um evento.  
		- Poderia ser para fazer um item coletável ao clicar.  

-----------------------