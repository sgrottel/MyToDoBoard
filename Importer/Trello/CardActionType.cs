using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Importer.Trello
{
	[JsonConverter(typeof(EnumJsonConverter<CardActionType>))]
	internal enum CardActionType
	{
		addAttachmentToCard,
		addChecklistToCard,
		addMemberToCard,
		addToOrganizationBoard,
		commentCard,
		convertToCardFromCheckItem,
		copyCard,
		createBoard,
		createCard,
		createList,
		deleteAttachmentFromCard,
		deleteCard,
		disablePlugin,
		enablePlugin,
		moveCardToBoard,
		removeChecklistFromCard,
		removeMemberFromCard,
		updateBoard,
		updateCard,
		updateCheckItemStateOnCard,
		updateList,
	}
}
