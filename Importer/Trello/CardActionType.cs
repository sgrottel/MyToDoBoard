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
		updateCard,
		createCard,
		commentCard,
		updateCheckItemStateOnCard,
		deleteCard,
		addAttachmentToCard,
		updateBoard,
		disablePlugin,
		copyCard,
		addChecklistToCard,
		moveCardToBoard,
		deleteAttachmentFromCard
	}
}
